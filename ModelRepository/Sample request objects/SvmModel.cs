using System.Text.Json.Serialization;

namespace ModelRepository.SampleRequestObjects
{
    public class SvmModel
    {
        [JsonPropertyName("0")]
        public double Value { get; set; }
    }
}
