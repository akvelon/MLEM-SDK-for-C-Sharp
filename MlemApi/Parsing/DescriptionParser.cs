using System.Text.Json;
using Microsoft.Extensions.Logging;
using MlemApi.Dto;
using MlemApi.MessageResources;
using MlemApi.Parsing.DataTypeParsers;
using MlemApi.Serializing;
using MlemApi.Validation.Exceptions;
using static System.Text.Json.JsonElement;

namespace MlemApi.Parsing
{
    /// <summary>
    /// Allows to parse api schema of mlem model
    /// </summary>
    internal class DescriptionParser
    {
        private IDataTypeProvider _dataTypeProvider = new DataTypeProvider();
        private ILogger? _logger;

        public DescriptionParser(ILogger logger)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Returns parsed api schema
        /// </summary>
        /// <param name="jsonStringDescription">Json representation of api schema from deployed mlem model</param>
        /// <returns>Parsed api schema</returns>
        /// <exception cref="InvalidApiSchemaException">Throws if schema is invalid</exception>
        public ApiDescription GetApiDescription(string jsonStringDescription)
        {
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonStringDescription);
            JsonElement jsonMethodElements = jsonDocument.RootElement.GetProperty("methods");
            _logger?.LogDebug("Parsing api description...");

            JsonElement.ObjectEnumerator jsonMethodElementsEnumerator = jsonMethodElements.EnumerateObject();
            ApiDescription description = new ApiDescription
            {
                Methods = new List<MethodDescription>(jsonMethodElementsEnumerator.Count())
            };

            _logger?.LogDebug("Trying to parse methods");
            foreach (var jsonMethodElement in jsonMethodElementsEnumerator)
            {
                try
                {
                    string? argsName = null;
                    ArgsData? argsData = null;
                    IApiDescriptionDataStructure? returnData = null;
                    var methodElement = jsonMethodElement.Value
                        .EnumerateObject();

                    var methodName = jsonMethodElement.Name;

                    try
                    {
                        var argsObject = methodElement.First(e => e.Name == "args").Value
                           .EnumerateArray().First()
                           .EnumerateObject();

                        argsName = argsObject.First(e => e.Name == "name").Value.GetString();
                        argsData = GetArgsData(argsObject, argsName);

                        var requestSerializerData = argsObject.First(e => e.Name == "serializer").Value.EnumerateObject();
                        var requestSerializerObject = GetRequestSerizlizeObject(requestSerializerData);
                        argsData.Serializer = requestSerializerObject;
                    }
                    catch (Exception e)
                    {
                        // todo - rewrite using logger
                        Console.WriteLine(string.Format(EM.ErrorDuringArgsParsing, methodName, e.Message));
                    }

                    try
                    {
                        var returnDataObject = jsonMethodElement.Value
                                .EnumerateObject().First(e => e.Name == "returns").Value.EnumerateObject();
                        returnData = GetReturnData(returnDataObject, argsName);
                    }
                    catch (Exception e)
                    {
                        // todo - rewrite using logger
                        Console.WriteLine(string.Format(EM.ErrorDurngReturnObjectParsing, methodName, e.Message));
                    }

                    description.Methods.Add(new MethodDescription(
                        methodName: jsonMethodElement.Name,
                        argsName,
                        argsData,
                        returnData
                    ));
                    _logger?.LogDebug($"Successfully parsed method {jsonMethodElement.Name}");
                }
                catch (Exception ex)
                {
                    throw new InvalidApiSchemaException(EM.InvalidApiSchema, ex);
                }
            }

            return description;
        }

        private IRequestValuesSerializer GetRequestSerizlizeObject(JsonElement.ObjectEnumerator? requestSerializeObjectEnumerator)
        {
            if (requestSerializeObjectEnumerator is not JsonElement.ObjectEnumerator notNullableArgsObjectEnumerator)
            {
                return new DefaultRequestValueSerializer();
            }

            _logger?.LogDebug("Parsing serializer...");

            var serializerType = notNullableArgsObjectEnumerator.First(e => e.Name == "type")
               .Value.ToString();

            return new DefaultRequestValueSerializer();
        }

        /// <summary>
        /// Parses schema for args data from relevant json object
        /// </summary>
        /// <param name="argsObjectEnumerator">json object enumerator for args data in api schema</param>
        /// <returns>Parsed schema for args data</returns>
        private ArgsData GetArgsData(JsonElement.ObjectEnumerator? argsObjectEnumerator, string argName)
        {
            if (argsObjectEnumerator is not JsonElement.ObjectEnumerator notNullableArgsObjectEnumerator)
            {
                return null;
            }

            _logger?.LogDebug("Parsing args data...");
            var typesDataObject = notNullableArgsObjectEnumerator.First(e => e.Name == $"{argName}_type")
                .Value.EnumerateObject();

            return new ArgsData
            {
                DataType = _dataTypeProvider.GetTypeFromSchema(typesDataObject, _dataTypeProvider)
            };
        }

        /// <summary>
        /// Parses schema for return data from relevant json object
        /// </summary>
        /// <param name="returnObjectEnumerator">json object enumerator for return data in api schema</param>
        /// <returns>Parsed schema for return data</returns>
        private IApiDescriptionDataStructure? GetReturnData(JsonElement.ObjectEnumerator? returnObjectEnumerator, string argName)
        {
            if (returnObjectEnumerator is not JsonElement.ObjectEnumerator notNullableReturnObjectEnumerator)
            {
                return null;
            }

            var typesDataObject = notNullableReturnObjectEnumerator.First(e => e.Name == $"{argName}_type")
                .Value.EnumerateObject();

            _logger?.LogDebug("Parsing return data...");
            return _dataTypeProvider.GetTypeFromSchema(typesDataObject, _dataTypeProvider);
        }
    }
}
