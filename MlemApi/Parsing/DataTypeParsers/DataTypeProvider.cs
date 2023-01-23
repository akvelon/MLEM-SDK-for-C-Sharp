using MlemApi.DataTypeParsers;
using MlemApi.Dto;
using MlemApi.MessageResources;
using System.Text.Json;

namespace MlemApi.Parsing.DataTypeParsers
{
    /// <summary>
    /// Root class for data type provides
    /// Encapsulates data type providers for all types currently supported by mlem client
    /// </summary>
    internal class DataTypeProvider : IDataTypeProvider
    {
        private readonly IList<IDataTypeProvider> _typeProviders = new List<IDataTypeProvider>()
        {
            new NumpyTypesProvider(),
            new PandasTypesProvider(),
            new PrimitiveTypesProvider(),
            new ListTypeProvider(),
        };

        private Dictionary<string, IDataTypeProvider> _supportedTypesProviderMap = new Dictionary<string, IDataTypeProvider>();

        public DataTypeProvider()
        {
            foreach (var typeProvider in _typeProviders)
            {
                typeProvider.GetSupportedTypes()
                    .ForEach((supportedType) => _supportedTypesProviderMap.Add(supportedType, typeProvider));
            }
        }

        public List<string> GetSupportedTypes()
        {
            return _supportedTypesProviderMap.Keys.ToList();
        }

        public IApiDescriptionDataStructure GetTypeFromSchema(JsonElement.ObjectEnumerator objectEnumerator, IDataTypeProvider childDataTypeProvider)
        {
            var dataType = objectEnumerator.First(e => e.Name == "type")
               .Value.ToString();

            IDataTypeProvider dataTypeProvider;

            var isTypeSupported = _supportedTypesProviderMap.TryGetValue(dataType, out dataTypeProvider);

            if (!isTypeSupported)
            {
                throw new ArgumentException(string.Format(EM.UnknownDataTypeInSchema, dataType));
            }

            return dataTypeProvider.GetTypeFromSchema(objectEnumerator, childDataTypeProvider);
        }
    }
}
