using System.Text.Json.Serialization;

namespace MlemApi.Dto
{
    internal class SklearnPredictRequest<T>
    {
        [JsonPropertyName("X")]
        public Data<T> Data { get; set; }
    }
}
