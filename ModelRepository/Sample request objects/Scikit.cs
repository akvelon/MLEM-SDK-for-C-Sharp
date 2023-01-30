using Newtonsoft.Json;

namespace ModelRepository.SampleRequestObjects
{
    public class Scikit
    {
        [JsonProperty("Pclass")]
        public long Pclass { get; set; }

        [JsonProperty("Parch")]
        public long Parch { get; set; }
    }
}