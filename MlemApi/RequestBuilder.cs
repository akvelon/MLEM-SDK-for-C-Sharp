namespace MlemApi
{
    /// <summary>
    /// 
    /// </summary>
    internal class RequestBuilder
    {
        private readonly IRequestValueSerializer _requestValueSerializer;

        public RequestBuilder(IRequestValueSerializer requestValueSerializer)
        {
            _requestValueSerializer = requestValueSerializer;
        }

        public string BuildRequest<T>(string argsName, IEnumerable<T> values)
        {
            var stringRequest = $"{{\"{argsName}\": " +
                "{\"values\": " +
                    "[" +
                        string.Join(',', _requestValueSerializer.Serialize(values)) +
                    "]" +
                "}" +
            "}";

            return stringRequest;
        }
    }
}
