using System.Text.Json;

namespace MlemApi
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class DefaultRequestValueSerializer : IRequestValueSerializer
    {
        public IEnumerable<string> Serialize<T>(IEnumerable<T> values)
        {
            return values.Select(v => JsonSerializer.Serialize(v));
        }
    }
}
