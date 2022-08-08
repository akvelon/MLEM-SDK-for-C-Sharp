using System.Text.Json.Serialization;

namespace MlemApi.Dto
{
    internal class Data<T>
    {
        [JsonPropertyName("values")]
        public List<T> Values { get; set; }
    }
}
