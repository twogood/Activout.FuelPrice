using System.Collections.Generic;
using System.Threading.Tasks;
using Activout.RestClient;
using Newtonsoft.Json;

namespace Activout.FuelPrice
{
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
        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }
        
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("text")]
        public string Text { get; set; }

        public override string ToString()
        {
            return $"{nameof(CreatedAt)}: {CreatedAt}, {nameof(Id)}: {Id}, {nameof(Text)}: {Text}";
        }
    }
}