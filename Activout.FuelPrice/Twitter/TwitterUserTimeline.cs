using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Activout.RestClient;
using Newtonsoft.Json;

namespace Activout.FuelPrice.Twitter
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class TwitterUserTimelineFactory
    {
        private readonly IRestClientFactory _restClientFactory;

        public TwitterUserTimelineFactory(IRestClientFactory restClientFactory)
        {
            _restClientFactory = restClientFactory;
        }

        public TwitterUserTimeline CreateTwitterUserTimeline(string accessToken)
        {
            var oauthHttpClient = new HttpClient();
            oauthHttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var userTimelineClient = _restClientFactory
                .CreateBuilder()
                .HttpClient(oauthHttpClient)
                .Build<IUserTimelineClient>();

            return new TwitterUserTimeline(userTimelineClient);
        }
    }

    public class TwitterUserTimeline
    {
        private readonly IUserTimelineClient _userTimelineClient;

        internal TwitterUserTimeline(IUserTimelineClient userTimelineClient)
        {
            _userTimelineClient = userTimelineClient;
        }

        public Task<List<UserTimelineEntry>> QueryByScreenName(string screenName)
        {
            return _userTimelineClient.QueryByScreenName(screenName);
        }

        public Task<List<UserTimelineEntry>> QueryByScreenName(string screenName, long maxId)
        {
            return _userTimelineClient.QueryByScreenNameAndMax(screenName, maxId);
        }
    }

    [InterfaceRoute("https://api.twitter.com/1.1/statuses/user_timeline.json")]
    [ErrorResponse(typeof(TwitterErrorResponse))]
    public interface IUserTimelineClient
    {
        Task<List<UserTimelineEntry>> QueryByScreenName([QueryParam("screen_name")] string screenName);

        Task<List<UserTimelineEntry>> QueryByScreenNameAndMax([QueryParam("screen_name")] string screenName,
            [QueryParam("max_id")] long maxId);
    }

    public class UserTimelineEntry
    {
        [JsonProperty("created_at")] public string CreatedAt { get; set; }

        [JsonProperty("id")] public long Id { get; set; }

        [JsonProperty("text")] public string Text { get; set; }

        public override string ToString()
        {
            return $"{nameof(CreatedAt)}: {CreatedAt}, {nameof(Id)}: {Id}, {nameof(Text)}: {Text}";
        }
    }
}