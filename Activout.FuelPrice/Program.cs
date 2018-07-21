using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Activout.RestClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Activout.FuelPrice
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddCommandLine(args)
                ;
            var configuration = builder.Build();

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddRestClient();
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var restClientFactory = serviceProvider.GetService<IRestClientFactory>();

            var twitterConfig = configuration.GetSection("twitter");
            var consumerKey = twitterConfig["consumerKey"];
            var consumerSecret = twitterConfig["consumerSecret"];

            if (string.IsNullOrWhiteSpace(consumerKey) || string.IsNullOrWhiteSpace(consumerSecret))
            {
                Console.Error.WriteLine("Missing configuration twitter:consumerKey and/or twitter:consumerSecret");
                return;
            }

            var basicAuthHttpClient = new HttpClient();
            basicAuthHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes($"{consumerKey}:{consumerSecret}")));
            var twitterAuth = restClientFactory
                .CreateBuilder()
                .HttpClient(basicAuthHttpClient)
                .Build<ITwitterAuth>();

            var token = twitterAuth.GetOauth2Token().Result;

            var oauthHttpClient = new HttpClient();
            oauthHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var userTimelineClient = restClientFactory
                .CreateBuilder()
                .HttpClient(oauthHttpClient)
                .Build<IUserTimelineClient>();

            var timeLine = userTimelineClient.QueryByScreenName("St1Sverige").Result;

            while (timeLine.Count != 0)
            {
                foreach (var entry in timeLine)
                {
                    if (!entry.Text.ToLower().Contains("ronneby")) continue;
                    Console.WriteLine(entry.CreatedAt);
                    Console.WriteLine(entry.Text);
                    return;
                }

                var maxId = timeLine[timeLine.Count - 1].Id - 1;
                timeLine = userTimelineClient.QueryByScreenNameAndMax("St1Sverige", maxId).Result;
            }
        }
    }
}