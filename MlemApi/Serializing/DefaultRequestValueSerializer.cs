namespace MlemApi.Serializing
{
    public class DefaultRequestValueSerializer : IRequestValuesSerializer
    {
        private DataFrameSerializer dataFrameSerializer = new DataFrameSerializer();
        private NdarraySerializer ndArraySerializer = new NdarraySerializer();

        private string GetRequestBody(string argsName, string argValue)
        {
            return $"{{\"{argsName}\": {argValue}}}";
        }

        public string Serialize<T>(IEnumerable<T> values, string argsName, string requestObjectType)
        {
            if (requestObjectType == "dataframe")
            {
                return GetRequestBody(
                    argsName,
                    $"{{" +
                        "\"values\": " +
                         $"[{string.Join(',', values.Select(value => dataFrameSerializer.Serialize(value)))}]" +
                    $"}}"
                );
            }
            else if (requestObjectType == "ndarray")
            {
                return GetRequestBody(
                    argsName,
                    $"[{string.Join(',', values.Select(value => ndArraySerializer.Serialize(value)))}]"
                );
            }
            else
            {
                throw new ArgumentException($"Unknown request object type - ${requestObjectType}");
            }
        }
    }
}
