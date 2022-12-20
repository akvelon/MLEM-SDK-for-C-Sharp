using Newtonsoft.Json;

namespace ModelRepository.InvalidRequestObjects
{
    internal class IrisWithInvalidArgumentType
    {
        [JsonProperty("sepal length (cm)")]
        public byte SepalLength { get; set; }

        [JsonProperty("sepal width (cm)")]
        public double SepalWidth { get; set; }

        [JsonProperty("petal length (cm)")]
        public double PetalLength { get; set; }

        [JsonProperty("petal width (cm)")]
        public double PetalWidth { get; set; }
    }
}
