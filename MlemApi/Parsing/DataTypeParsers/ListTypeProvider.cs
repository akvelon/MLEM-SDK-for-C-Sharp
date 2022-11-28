using MlemApi.Dto;
using System.Text.Json;

namespace MlemApi.Parsing.DataTypeParsers
{
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
            });

            return new ListData() {
                Items = typesList,
            };
        }
    }
}
