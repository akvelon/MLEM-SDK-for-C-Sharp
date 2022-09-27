using MlemApi;
using Newtonsoft.Json.Linq;

namespace MlemApiClientTests
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class NewtonsoftRequestValueSerializer : IRequestValueSerializer
    {
        public IEnumerable<string> Serialize<T>(IEnumerable<T> values)
        {
            return values.Select(v => JObject.FromObject(v).ToString());
        }
    }
}
