using System.Text.Json.Serialization;

namespace MlemApiClientTests.RegModelTests
{
    public class RegModel
    {
        [JsonPropertyName("0")]
        public double Zero { get; set; }
    }
}
