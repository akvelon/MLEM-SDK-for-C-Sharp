using Newtonsoft.Json.Linq;

namespace MlemApi.Serializing
{
    public class DataFrameSerializer : IRequestValueSerializer
    {
        public string Serialize<T>(T value) => JObject.FromObject(value).ToString();
    }
}
