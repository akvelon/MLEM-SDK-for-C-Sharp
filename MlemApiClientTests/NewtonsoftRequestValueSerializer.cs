using MlemApi;
using Newtonsoft.Json.Linq;

namespace MlemApiClientTests
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class NewtonsoftRequestValueSerializer : IRequestValueSerializer
    {
        public List<string> Serialize<T>(List<T> values)
        {
            return values.Select(v => JObject.FromObject(v).ToString()).ToList();
        }
    }
}
