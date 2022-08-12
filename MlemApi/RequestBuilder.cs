using Newtonsoft.Json.Linq;

namespace MlemApi
{
    internal static class RequestBuilder<T>
    {
        public static string BuildRequest(string argsName, List<T> values)
        {
            var jsonRequest = new JObject(
                    new JProperty($"{argsName}", new JObject(
                        new JProperty("values", JArray.FromObject(values))))
                );

            return jsonRequest.ToString();
        }
    }
}
