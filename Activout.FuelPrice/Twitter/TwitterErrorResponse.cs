using System.Collections.Generic;
using Newtonsoft.Json;

namespace Activout.FuelPrice.Twitter
{
    public class TwitterErrorResponse
    {
        [JsonProperty("errors")]
        public List<TwitterError> Errors{ get; set; }

        public override string ToString()
        {
            return $"{nameof(Errors)}: {string.Join(",",Errors)}";
        }
    }
}