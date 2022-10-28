using MlemApi.Serializing;

namespace MlemApi
{
    /// <summary>
    /// 
    /// </summary>
    public class RequestBuilder
    {
        private readonly IRequestValuesSerializer _requestValueSerializer;

        public RequestBuilder(IRequestValuesSerializer requestValueSerializer)
        {
            _requestValueSerializer = requestValueSerializer;
        }

        public string BuildRequest<T>(string argsName, IEnumerable<T> values, string requestObjectType)
        {
            var stringRequest = _requestValueSerializer.Serialize(values, argsName, requestObjectType);

            return stringRequest;
        }
    }
}
