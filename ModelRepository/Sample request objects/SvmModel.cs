using System.Text.Json.Serialization;

namespace ModelRepository.SampleRequestObjects
{
    internal class SvmModel
    {
        [JsonPropertyName("0")]
        public double Value { get; set; }
    }
}
