using System.Text.Json;
using MlemApi.DataTypeParsers;
using MlemApi.Dto;
using MlemApi.Parsing.DataTypeParsers;
using MlemApi.Validation.Exceptions;

namespace MlemApi.Parsing
{
    internal class DescriptionParser
    {
        private IDataTypeProvider dataTypeProvider = new DataTypeProvider();

        public ApiDescription GetApiDescription(string jsonStringDescription)
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
                try
                {
                    var methodElement = jsonMethodElement.Value
                        .EnumerateObject();

                    var argsArrayObject = methodElement.First(e => e.Name == "args").Value
                           .EnumerateArray();

                    IApiDescriptionDataStructure? argsData = null;
                    string? argsName = null;

                    if (argsArrayObject.Count() != 0)
                    {
                        var argsObject = methodElement.First(e => e.Name == "args").Value
                           .EnumerateArray().First()
                           .EnumerateObject();

                        argsData = GetArgsData(argsObject);
                        argsName = argsObject.First(e => e.Name == "name").Value.GetString();
                    }

                    var returnDataObject = jsonMethodElement.Value
                            .EnumerateObject().First(e => e.Name == "returns").Value.EnumerateObject();

                    description.Methods.Add(new MethodDescription(
                        jsonMethodElement.Name,
                        argsName,
                        argsData,
                        GetReturnData(returnDataObject) as NdarrayData
                    ));
                }
                catch (Exception ex)
                {
                    throw new InvalidApiSchemaException("Invalid api schema", ex);
                }
            }

            return description;
        }

        private IApiDescriptionDataStructure GetArgsData(JsonElement.ObjectEnumerator argsObjectEnumerator)
        {
            var typesDataObject = argsObjectEnumerator.First(e => e.Name == "type_")
                .Value.EnumerateObject();

            IApiDescriptionDataStructure dataType = dataTypeProvider.GetTypeFromSchema(typesDataObject, dataTypeProvider);

            if (dataType is NdarrayData)
            {
                var ndArrayType = dataType as NdarrayData;
                var shapeList = ndArrayType.Shape as List<int?>;
                ndArrayType.Shape = shapeList;
            }

            return dataType;
        }

        private IApiDescriptionDataStructure GetReturnData(JsonElement.ObjectEnumerator returnObjectEnumerator)
        {
            return dataTypeProvider.GetTypeFromSchema(returnObjectEnumerator, dataTypeProvider);
        }
    }
}
