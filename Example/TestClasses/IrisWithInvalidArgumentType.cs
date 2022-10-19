using ModelGenerator;
using Newtonsoft.Json;

namespace Example
{
    public class IrisWithInvalidArgumentType : RequestModelType
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
