using MlemApi.Dto;
using System.Text.Json;

namespace MlemApi.Parsing.DataTypeParsers
{
    /// <summary>
    /// Gets Array type from schema
    /// See relevant mlem code - https://github.com/iterative/mlem/blob/663862f233c6422c7a27fef682eb8f1a3d4a46ea/mlem/core/data_type.py#L249
    /// </summary>
    internal class ArrayTypeProvider : IDataTypeProvider
    {
        public List<string> GetSupportedTypes()
        {
            return new List<string>() { "array" };
        }

        public IApiDescriptionDataStructure GetTypeFromSchema(JsonElement.ObjectEnumerator objectEnumerator, IDataTypeProvider childDataTypeProvider)
        {
            var arrayItems = objectEnumerator.First(e => e.Name == "items")
               .Value.EnumerateArray()
               .ToList();

            var typeArray = arrayItems.Select(item =>
            {
                return childDataTypeProvider.GetTypeFromSchema(item.EnumerateObject(), childDataTypeProvider);
            }).ToList();

            return new ListData()
            {
                Items = typeArray,
            };
        }
    }
}