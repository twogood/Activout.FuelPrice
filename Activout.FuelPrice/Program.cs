using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Activout.FuelPrice.Twitter;
using Activout.RestClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Activout.FuelPrice
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = GetConfig(args);
            var serviceProvider = GetServiceProvider();

            var twitterConfig = configuration.GetSection("twitter");
            var consumerKey = twitterConfig["consumerKey"];
            var consumerSecret = twitterConfig["consumerSecret"];

            if (string.IsNullOrWhiteSpace(consumerKey) || string.IsNullOrWhiteSpace(consumerSecret))
            {
                Console.Error.WriteLine("Missing configuration twitter:consumerKey and/or twitter:consumerSecret");
                return;
            }

            var twitterAuth = serviceProvider.GetService<TwitterAuth>();
            var accessToken = twitterAuth.GetAccessToken(consumerKey, consumerSecret).Result;
       
            var userTimelineClientFactory = serviceProvider.GetService<TwitterUserTimelineFactory>();
            var userTimelineClient = userTimelineClientFactory.CreateTwitterUserTimeline(accessToken);

            const string screenName = "St1Sverige";
            const string hashtag = "#st1ronneby";
            
            var timeLine = userTimelineClient.QueryByScreenName(screenName).Result;
            while (timeLine.Count != 0)
            {
                foreach (var entry in timeLine)
                {
                    if (!entry.Text.ToLower().Contains(hashtag)) continue;
                    Console.WriteLine(entry.CreatedAt);
                    Console.WriteLine(entry.Text);
                    return;
                }

                var maxId = timeLine[timeLine.Count - 1].Id - 1;
                timeLine = userTimelineClient.QueryByScreenName(screenName, maxId).Result;
            }
        }

        private static ServiceProvider GetServiceProvider()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddRestClient();
            serviceCollection.AddSingleton<TwitterAuth>();
            serviceCollection.AddSingleton<TwitterUserTimelineFactory>();
            return serviceCollection.BuildServiceProvider();
        }

        private static IConfigurationRoot GetConfig(string[] args)
        {
            var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddCommandLine(args)
                ;
            return builder.Build();
        }
    }
}