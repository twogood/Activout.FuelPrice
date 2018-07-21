using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Activout.RestClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Activout.FuelPrice.Twitter
{
    [InterfaceRoute("https://api.twitter.com")]
    [ErrorResponse(typeof(TwitterErrorResponse))]
    public interface ITwitterAuthClient
    {
        [HttpPost("/oauth2/token")]
        Task<Oauth2Token> GetOauth2Token([FormParam("grant_type")] string grantType = "client_credentials");
    }

    public class Oauth2Token
    {
        [JsonProperty("token_type")] public string TokenType { get; set; }
        [JsonProperty("access_token")] public string AccessToken { get; set; }
    }
    
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class TwitterAuth
    {
        private readonly IRestClientFactory _restClientFactory;

        public TwitterAuth(IRestClientFactory restClientFactory)
        {
            _restClientFactory = restClientFactory;
        }
        
        public async Task<string> GetAccessToken(string consumerKey, string consumerSecret)
        {
            var basicAuthHttpClient = new HttpClient();
            basicAuthHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic",
                    Convert.ToBase64String(Encoding.UTF8.GetBytes($"{consumerKey}:{consumerSecret}")));
            var twitterAuth = _restClientFactory
                .CreateBuilder()
                .HttpClient(basicAuthHttpClient)
                .Build<ITwitterAuthClient>();

            return (await twitterAuth.GetOauth2Token()).AccessToken;
        }
    }
}