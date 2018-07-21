using System.Threading.Tasks;
using Activout.RestClient;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Activout.FuelPrice
{
    [InterfaceRoute("https://api.twitter.com")]
    [ErrorResponse(typeof(TwitterErrorResponse))]
    public interface ITwitterAuth
    {
        [HttpPost("/oauth2/token")]
        Task<Oauth2Token> GetOauth2Token([FormParam("grant_type")] string grantType = "client_credentials");
    }

    public class Oauth2Token
    {
        [JsonProperty("token_type")] public string TokenType { get; set; }
        [JsonProperty("access_token")] public string AccessToken { get; set; }
    }
}