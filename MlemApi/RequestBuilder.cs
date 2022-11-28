using MlemApi.Dto;
using MlemApi.Serializing;

namespace MlemApi
{
    public class RequestBuilder
    {
        private readonly IRequestValuesSerializer _requestValueSerializer;

        public RequestBuilder(IRequestValuesSerializer requestValueSerializer)
        {
            _requestValueSerializer = requestValueSerializer;
        }

        public string BuildRequest<T>(string argsName, IEnumerable<T> values, Type argsType)
            => _requestValueSerializer.Serialize(values, argsName, argsType);
    }
}
