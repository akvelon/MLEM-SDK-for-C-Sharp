using System.Text.Json;
using MlemApi.Dto;

namespace MlemApi
{
    internal static class DescriptionParser
    {
        public static ApiDescription GetApiDescription(string jsonStringDescription)
        { 
            using var jsonDocument = JsonDocument.Parse(jsonStringDescription);
            var jsonMethodElements = jsonDocument.RootElement.GetProperty("methods");

            var jsonMethodElementsEnumerator = jsonMethodElements.EnumerateObject();
            var description = new ApiDescription
            {
                Methods = new List<MethodDescription>(jsonMethodElementsEnumerator.Count())
            };

            foreach (var jsonMethodElement in jsonMethodElementsEnumerator)
            {
                description.Methods.Add(new MethodDescription
                {
                    MethodName = jsonMethodElement.Name,
                    ArgsName = jsonMethodElement.Value
                        .EnumerateObject().First(e => e.Name == "args").Value
                        .EnumerateArray().First()
                        .EnumerateObject().First(e => e.Name == "name").Value.GetString()
                });
            }

            return description;
        }
    }
}
