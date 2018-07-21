using Newtonsoft.Json;

namespace Activout.FuelPrice.Twitter
{
    public class TwitterError
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }

        public override string ToString()
        {
            return $"{nameof(Code)}: {Code}, {nameof(Message)}: {Message}";
        }
    }
}