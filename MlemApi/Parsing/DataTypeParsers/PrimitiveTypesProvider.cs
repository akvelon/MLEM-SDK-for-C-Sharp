using MlemApi.Dto;
using MlemApi.Utils;
using System.Text.Json;

namespace MlemApi.Parsing.DataTypeParsers
{
    /// <summary>
    /// Gets primitive types from schema
    /// See relevant mlem code - https://github.com/iterative/mlem/blob/a891636ff1f464d11925acaa845eb9f6c8efc044/mlem/core/data_type.py
    /// </summary>
    internal class PrimitiveTypesProvider : IDataTypeProvider
    {
        private IPrimitiveTypeHelper primitiveTypeHelper = new PrimitiveTypeHelper();
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
