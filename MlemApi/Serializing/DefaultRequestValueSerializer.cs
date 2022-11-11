namespace MlemApi.Serializing
{
    public class DefaultRequestValueSerializer : IRequestValuesSerializer
    {
        private readonly DataFrameSerializer _dataFrameSerializer = new();
        private readonly NdarraySerializer _ndArraySerializer = new();

        private string GetRequestBody(string argsName, string argValue)
            => $"{{\"{argsName}\": {argValue}}}";

        public string Serialize<T>(IEnumerable<T> values, string argsName, string requestObjectType)
            => requestObjectType switch
            {
                "dataframe" => GetRequestBody(
                    argsName,
                    $"{{" +
                        "\"values\": " +
                         $"[{string.Join(',', values.Select(value => _dataFrameSerializer.Serialize(value)))}]" +
                    $"}}"
                ),
                "ndarray" => GetRequestBody(
                    argsName,
                    $"[{string.Join(',', values.Select(value => _ndArraySerializer.Serialize(value)))}]"
                ),
                _ => throw new ArgumentException($"Unknown request object type - ${requestObjectType}")
            };
    }
}
