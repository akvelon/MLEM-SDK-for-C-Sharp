using Newtonsoft.Json.Linq;

namespace MlemApi.Serializing
{
    public class DataFrameSerializer : IRequestValueSerializer
    {
        public string Serialize<T>(T value)
        {
            return JObject.FromObject(value).ToString();
        }
    }
}
