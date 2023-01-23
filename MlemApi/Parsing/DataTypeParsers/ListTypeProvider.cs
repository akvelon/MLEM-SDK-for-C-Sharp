using MlemApi.Dto;
using System.Text.Json;

namespace MlemApi.Parsing.DataTypeParsers
{
    /// <summary>
    /// Gets List type from schema
    /// See relevant mlem code - https://github.com/iterative/mlem/blob/a891636ff1f464d11925acaa845eb9f6c8efc044/mlem/core/data_type.py#L654
    /// </summary>
    internal class ListTypeProvider : IDataTypeProvider
    {
        public List<string> GetSupportedTypes()
        {
            return new List<string>() { "list"};
        }

        public IApiDescriptionDataStructure GetTypeFromSchema(JsonElement.ObjectEnumerator objectEnumerator, IDataTypeProvider childDataTypeProvider)
        {
            var listItems = objectEnumerator.First(e => e.Name == "items")
               .Value.EnumerateArray()
               .ToList();

            var typesList = listItems.Select(item =>
            {
                return childDataTypeProvider.GetTypeFromSchema(item.EnumerateObject(), childDataTypeProvider);
            }).ToList();

            return new ListData() {
                Items = typesList,
            };
        }
    }
}
