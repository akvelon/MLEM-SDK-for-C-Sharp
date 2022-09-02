using System.Text.Json.Serialization;

namespace Example
{
    public class SvmModel
    {
        [JsonPropertyName("0")]
        public double Zero { get; set; }
    }
}
