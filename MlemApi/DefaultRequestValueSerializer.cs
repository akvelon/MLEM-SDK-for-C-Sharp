using Newtonsoft.Json.Linq;

namespace MlemApi
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class DefaultRequestValueSerializer : IRequestValueSerializer
    {
        public IEnumerable<string> Serialize<T>(IEnumerable<T> values)
        {
            return values.Select(v => JObject.FromObject(v).ToString());
        }
    }
}
