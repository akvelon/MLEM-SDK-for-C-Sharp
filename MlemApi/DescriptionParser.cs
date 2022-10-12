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
                var argsObject = jsonMethodElement.Value
                        .EnumerateObject().First(e => e.Name == "args").Value
                        .EnumerateArray().First()
                        .EnumerateObject();

                description.Methods.Add(new MethodDescription
                {
                    MethodName = jsonMethodElement.Name,
                    ArgsName = argsObject.First(e => e.Name == "name").Value.GetString(),
                    ArgsData = DescriptionParser.GetArgsData(argsObject),

                });
            }

            return description;
        }

        private static IEnumerable<MethodArgumentData> GetArgsData(JsonElement.ObjectEnumerator argsObjectEnumerator)
        {
            var typesDataObject = argsObjectEnumerator.First(e => e.Name == "type_")
                .Value.EnumerateObject();

            var argumentNames = typesDataObject.First(e => e.Name == "columns")
                .Value.EnumerateArray()
                .Select(element => element.GetString())
                .ToList();

            var argumentTypes = typesDataObject.First(e => e.Name == "dtypes")
                .Value.EnumerateArray()
                .Select(element => element.GetString())
                .ToList();

            return argumentNames.Select((argumentName, index) => new MethodArgumentData
            {
                ArgumentName = argumentName,
                ArgumentType = argumentTypes[index]
            });
        }
    }
}
