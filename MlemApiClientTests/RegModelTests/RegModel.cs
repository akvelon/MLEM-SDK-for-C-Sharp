using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace MlemApiClientTests.RegModelTests
{
    public class RegModel
    {
        [JsonProperty("0")]
        public double Zero { get; set; }
    }
}
