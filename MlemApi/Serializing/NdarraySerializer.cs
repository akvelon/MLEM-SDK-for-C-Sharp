using Newtonsoft.Json.Linq;

namespace MlemApi.Serializing
{
    /// <summary>
    /// Serializes ndarray type to json
    /// </summary>
    public class NdarraySerializer : IRequestValueSerializer
    {
        public string Serialize<T>(T value) => JArray.FromObject(value).ToString();
    }
}
