using MlemApi.Dto;
using System.Text.Json;

namespace MlemApi.Parsing.DataTypeParsers
{
    /// <summary>
    /// Parses primitive data types
    /// </summary>
    internal class PrimitiveTypesProvider : IDataTypeProvider
    {
        public List<string> GetSupportedTypes()
        {
            return new List<string>() { "primitive" };
        }

        public IApiDescriptionDataStructure GetTypeFromSchema(JsonElement.ObjectEnumerator objectEnumerator, IDataTypeProvider childDataTypeProvider)
        {
            var primitiveTypeName = objectEnumerator.First(e => e.Name == "ptype")
                .Value.ToString();

            return new PrimitiveData()
            {
                Ptype = primitiveTypeName,
            };
        }
    }
}
