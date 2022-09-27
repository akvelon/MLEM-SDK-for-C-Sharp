using System.Text.Json;
using System.Text.Json.Serialization;

namespace MlemApiClientTests.SimplifiedClientTests
{
    public class Iris
    {
        [JsonPropertyName("sepal length (cm)")]
        public double SepalLength { get; set; }

        [JsonPropertyName("sepal width (cm)")]
        public double SepalWidth { get; set; }

        [JsonPropertyName("petal length (cm)")]
        public double PetalLength { get; set; }

        [JsonPropertyName("petal width (cm)")]
        public double PetalWidth { get; set; }
    }
}
