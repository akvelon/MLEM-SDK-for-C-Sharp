using Newtonsoft.Json.Linq;

namespace MlemApi.Serializing
{
    public class NdarraySerializer : IRequestValueSerializer
    {
        public string Serialize<T>(T value) => JArray.FromObject(value).ToString();
    }
}
