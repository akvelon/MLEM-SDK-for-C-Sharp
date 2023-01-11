using MlemApi.Dto;
using MlemApi.Dto.DataFrameData;
using System.Net.Mime;
using System.Text;

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
            else if (argsType == typeof(DataFrameData) || argsType == typeof(ListData))
            {
                return GetRequestBody(
                    argsName,
                    _arraySerializer.Serialize(values.First())
                );
            } else
            {
                return GetRequestBody(
                    argsName,
                    values.First().ToString()
                );
            }
        }

        HttpContent IRequestValuesSerializer.BuildRequest<T>(string argsName, IEnumerable<T> values, Type argsType)
        {
            var serializedObject = Serialize(values, argsName, argsType);
            return new StringContent(serializedObject, Encoding.UTF8, MediaTypeNames.Application.Json);
        }
    }
}
