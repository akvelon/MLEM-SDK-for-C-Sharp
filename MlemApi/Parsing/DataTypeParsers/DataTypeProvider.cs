using MlemApi.DataTypeParsers;
using MlemApi.Dto;
using System.Text.Json;

namespace MlemApi.Parsing.DataTypeParsers
{
    internal class DataTypeProvider : IDataTypeProvider
    {
        private readonly IList<IDataTypeProvider> typeProviders = new List<IDataTypeProvider>()
        {
            new NumpyTypesProvider(),
            new PandasTypesProvider(),
            new PrimitiveTypesProvider(),
            new ListTypeProvider(),
        };

        private Dictionary<string, IDataTypeProvider> supportedTypesProviderMap = new Dictionary<string, IDataTypeProvider>();

        public DataTypeProvider()
        {
            foreach (var typeProvider in typeProviders)
            {
                typeProvider.GetSupportedTypes()
                    .ForEach((supportedType) => supportedTypesProviderMap.Add(supportedType, typeProvider));
            }
        }

        public List<string> GetSupportedTypes()
        {
            return supportedTypesProviderMap.Keys.ToList();
        }

        public IApiDescriptionDataStructure GetTypeFromSchema(JsonElement.ObjectEnumerator objectEnumerator, IDataTypeProvider childDataTypeProvider)
        {
            var dataType = objectEnumerator.First(e => e.Name == "type")
               .Value.ToString();

            IDataTypeProvider dataTypeProvider;

            var isTypeSupported = supportedTypesProviderMap.TryGetValue(dataType, out dataTypeProvider);

            if (!isTypeSupported)
            {
                throw new ArgumentException($"Uknown data type in schema: {dataType}");
            }

            return dataTypeProvider.GetTypeFromSchema(objectEnumerator, childDataTypeProvider);
        }
    }
}
