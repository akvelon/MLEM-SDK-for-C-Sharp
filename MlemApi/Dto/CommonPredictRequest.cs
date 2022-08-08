using System.Text.Json.Serialization;

namespace MlemApi.Dto
{
    internal class CommonPredictRequest<T>
    {
        [JsonPropertyName("data")]
        public Data<T> Data { get; set; }
    }
}
