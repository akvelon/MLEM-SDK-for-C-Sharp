using System.Text.Json.Serialization;

namespace ModelGenerator.Example
{
    internal class SvmModel
    {
        [JsonPropertyName("0")]
        public double Value { get; set; }
    }
}
