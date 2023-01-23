using Newtonsoft.Json.Linq;

namespace MlemApi.Serializing
{
    /// <summary>
    /// Serializes dataframe data type to json
    /// </summary>
    public class DataFrameSerializer : IRequestValueSerializer
    {
        public string Serialize<T>(T value) => JObject.FromObject(value).ToString();
    }
}
