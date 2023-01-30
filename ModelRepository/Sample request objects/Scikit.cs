using Newtonsoft.Json;

namespace ModelRepository.SampleRequestObjects
{
    public class Scikit
    {
        [JsonProperty("pclass")]
        public long Pclass { get; set; }

        [JsonProperty("parch")]
        public long Parch { get; set; }
    }
}