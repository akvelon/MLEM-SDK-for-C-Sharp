using System.Text.Json;
using Microsoft.Extensions.Logging;
using MlemApi.DataTypeParsers;
using MlemApi.Dto;
using MlemApi.MessageResources;
using MlemApi.Parsing.DataTypeParsers;
using MlemApi.Validation.Exceptions;

namespace MlemApi.Parsing
{
    internal class DescriptionParser
    {
        private IDataTypeProvider dataTypeProvider = new DataTypeProvider();
        private ILogger? logger;

        public DescriptionParser(ILogger logger)
        {
            this.logger = logger;
        }

        public ApiDescription GetApiDescription(string jsonStringDescription)
        {
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonStringDescription);
            JsonElement jsonMethodElements = jsonDocument.RootElement.GetProperty("methods");
            logger?.LogDebug("Parsing api description...");

            JsonElement.ObjectEnumerator jsonMethodElementsEnumerator = jsonMethodElements.EnumerateObject();
            ApiDescription description = new ApiDescription
            {
                Methods = new List<MethodDescription>(jsonMethodElementsEnumerator.Count())
            };

            logger?.LogDebug("Trying to parse methods");
            foreach (var jsonMethodElement in jsonMethodElementsEnumerator)
            {
                try
                {
                    string? argsName = null;
                    IApiDescriptionDataStructure? argsData = null;
                    NdarrayData? returnData = null;
                    var methodElement = jsonMethodElement.Value
                        .EnumerateObject();

                    var methodName = jsonMethodElement.Name;

                    try
                    {
                        var argsObject = methodElement.First(e => e.Name == "args").Value
                           .EnumerateArray().First()
                           .EnumerateObject();

                        argsData = GetArgsData(argsObject);
                        argsName = argsObject.First(e => e.Name == "name").Value.GetString();
                    }
                    catch (Exception e)
                    {
                        // todo - rewrite using logger
                        Console.WriteLine($"Error during args parsing for method {methodName}: {e.Message}. Args will be considered as empty.");
                    }

                    try
                    {
                        var returnDataObject = jsonMethodElement.Value
                                .EnumerateObject().First(e => e.Name == "returns").Value.EnumerateObject();
                        returnData = GetReturnData(returnDataObject) as NdarrayData;
                    }
                    catch (Exception e)
                    {
                        // todo - rewrite using logger
                        Console.WriteLine($"Error during return object schema parsing for method {methodName}: {e.Message}. Return object schema will be considered as empty.");
                    }

                    description.Methods.Add(new MethodDescription(
                        methodName: jsonMethodElement.Name,
                        argsName,
                        argsData,
                        returnData
                    ));
                    logger?.LogDebug($"Successfully parsed method {jsonMethodElement.Name}");
                }
                catch (Exception ex)
                {
                    throw new InvalidApiSchemaException(EM.InvalidApiSchema, ex);
                }
            }

            return description;
        }

        private IApiDescriptionDataStructure? GetArgsData(JsonElement.ObjectEnumerator? argsObjectEnumerator)
        {
            if (argsObjectEnumerator is not JsonElement.ObjectEnumerator notNullableArgsObjectEnumerator)
            {
                return null;
            }

            logger?.LogDebug("Parsing args data...");
            var typesDataObject = notNullableArgsObjectEnumerator.First(e => e.Name == "type_")
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

        private IApiDescriptionDataStructure? GetReturnData(JsonElement.ObjectEnumerator? returnObjectEnumerator)
        {
            if (returnObjectEnumerator is not JsonElement.ObjectEnumerator notNullableReturnObjectEnumerator)
            {
                return null;
            }

            logger?.LogDebug("Parsing return data...");
            return dataTypeProvider.GetTypeFromSchema(notNullableReturnObjectEnumerator, dataTypeProvider);
        }
    }
}
