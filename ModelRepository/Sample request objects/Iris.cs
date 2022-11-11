using Newtonsoft.Json;

namespace ModelRepository.SampleRequestObjects
{
    internal class Iris
    {
        [JsonProperty("sepal length (cm)")]
        public double SepalLength { get; set; }

        [JsonProperty("sepal width (cm)")]
        public double SepalWidth { get; set; }

        [JsonProperty("petal length (cm)")]
        public double PetalLength { get; set; }

        [JsonProperty("petal width (cm)")]
        public double PetalWidth { get; set; }
    }
}
