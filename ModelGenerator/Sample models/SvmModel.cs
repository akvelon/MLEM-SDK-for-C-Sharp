using System.Text.Json.Serialization;

namespace ModelGenerator.Example
{
    internal class SvmModel : Model
    {
        [JsonPropertyName("0")]
        public double Value { get; set; }
    }
}
