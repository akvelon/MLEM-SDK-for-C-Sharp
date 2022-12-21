using MlemApi.Dto.DataFrameData;
using MlemApi.MessageResources;

namespace MlemApi.Serializing
{
    public class DefaultRequestValueSerializer : IRequestValuesSerializer
    {
        private readonly DataFrameSerializer _dataFrameSerializer = new();
        private readonly NdarraySerializer _ndArraySerializer = new();

        private string GetRequestBody(string argsName, string argValue)
            => $"{{\"{argsName}\": {argValue}}}";

        public string Serialize<T>(IEnumerable<T> values, string argsName, Type argsType)
        {
            if (argsType == typeof(DataFrameData))
            {
                return GetRequestBody(
                    argsName,
                    $"{{" +
                        "\"values\": " +
                         $"[{string.Join(',', values.Select(value => _dataFrameSerializer.Serialize(value)))}]" +
                    $"}}"
                );
            }
            else
            {
                return GetRequestBody(
                    argsName,
                    _ndArraySerializer.Serialize(values.First())
                );
            }
        }
    }
}
