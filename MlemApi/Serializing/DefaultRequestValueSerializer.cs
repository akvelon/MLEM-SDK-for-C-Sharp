using MlemApi.Dto;
using MlemApi.Dto.DataFrameData;
using Newtonsoft.Json;

namespace MlemApi.Serializing
{
    public class DefaultRequestValueSerializer : IRequestValuesSerializer
    {
        private readonly DataFrameSerializer _dataFrameSerializer = new();
        private readonly ArraySerializer _arraySerializer = new();

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
            else if (argsType == typeof(ListData))
            {
                return GetRequestBody(
                    argsName,
                    _arraySerializer.Serialize(values.First())
                );
            } else
            {
                return GetRequestBody(
                    argsName,
                    JsonConvert.SerializeObject(values)
                );
            }
        }
    }
}
