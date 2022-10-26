using System.Collections;
using System.Data;
using Microsoft.Extensions.Logging;
using MlemApi.Dto;
using MlemApi.Dto.DataFrameArgumentData;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MlemApi
{
    internal class Validator : IValidator
    {
        private readonly ILogger? _logger;
        private readonly ApiDescription _apiDescription;

        /// <summary>
        /// Map of Numpy types (used in sklearn) to C# primitive types
        /// See https://gist.github.com/robbmcleod/73ca42da5984e6d0e5b6ad28bc4a504efor for the referenced list of types
        /// </summary>
        private readonly Dictionary<string, string> typesMap = new Dictionary<string, string>()
        {
            { "float32", "Single" },
            { "float64", "Double" },
            { "int8", "SByte" },
            { "int16", "Int16" },
            { "int32", "Int32" },
            { "int64", "Int64" },
            { "uint8", "Byte" },
            { "uint16", "UInt16" },
            { "uint32", "UInt32" },
            { "uint64", "UInt64" },
            { "bool", "Boolean" },
        };

        public Validator(ApiDescription _apiDescription, ILogger logger = null)
        {
            this._logger = logger;
            this._apiDescription = _apiDescription;
        }

        public void ValidateMethod(string methodName)
        {
            if (!_apiDescription.Methods.Any(m => m.MethodName == methodName))
            {
                var message = $"No method {methodName} in API.";

                _logger?.LogError(message);

                throw new InvalidOperationException(message);
            }
        }

        public void ValidateValues<incomeT>(IEnumerable<incomeT> values,
            string methodName,
            bool argumentTypesValidationIsOn = true,
            Dictionary<string, string> modelColumnToPropNamesMap = null
        )
        {
            if (values == null)
            {
                _logger?.LogError($"Input value is null: {nameof(values)}.");

                throw new ArgumentNullException(nameof(values));
            }

            if (!values.Any())
            {
                _logger?.LogError($"Input value is empty: {nameof(values)}.");

                throw new ArgumentException($"{nameof(values)} cannot be empty.");
            }

            if (argumentTypesValidationIsOn)
            {
                foreach (var value in values)
                {
                    ValidateArgument(value, methodName, modelColumnToPropNamesMap);
                }
            }
        }

        public void ValidateJsonResponse(string response, string methodName)
        {
            var methodReturnDataSchema = GetMethodDescriptionFromSchema(methodName)?.ReturnData;

            try
            {
                var parsedResponse = JArray.Parse(response);

                // Objects queue with nesting level
                var listElementsQueue = new Queue<Tuple<object, int>>();
                listElementsQueue.Enqueue(Tuple.Create<object, int>(parsedResponse, 0));

                ValidateNdarrayData<JArray, JArray>(parsedResponse, methodReturnDataSchema);
            }
            catch (JsonReaderException e)
            {
                throw new JsonReaderException($"Invalid json response from model - {e.Message}");
            }
        }

        private void ValidateStringifiedValue(object jsonValue, string expectedNumPyTypeName)
        {
            string expectedTypeName;
            var valueString = jsonValue.ToString();

            try
            {
                expectedTypeName = this.typesMap[expectedNumPyTypeName];
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException($"Unknown value type - {expectedNumPyTypeName}");
            }

            try
            {
                switch (expectedTypeName)
                {
                    case "Double":
                        {
                            Double.Parse(valueString);
                            break;
                        }
                    case "Single":
                        {
                            Single.Parse(valueString);
                            break;
                        }
                    case "SByte":
                        {
                            SByte.Parse(valueString);
                            break;
                        }
                    case "Int16":
                        {
                            Int16.Parse(valueString);
                            break;
                        }
                    case "Int32":
                        {
                            Int32.Parse(valueString);
                            break;
                        }
                    case "Int64":
                        {
                            Int64.Parse(valueString);
                            break;
                        }
                    case "Byte":
                        {
                            Byte.Parse(valueString);
                            break;
                        }
                    case "UInt16":
                        {
                            UInt16.Parse(valueString);
                            break;
                        }
                    case "UInt32":
                        {
                            UInt32.Parse(valueString);
                            break;
                        }
                    case "UInt64":
                        {
                            UInt64.Parse(valueString);
                            break;
                        }
                    case "Boolean":
                        {
                            Boolean.Parse(valueString);
                            break;
                        }
                    default:
                        {
                            throw new Exception($"No validation logic for type {expectedTypeName}");
                            break;
                        }
                }
            }
            catch (FormatException)
            {
                throw new FormatException($"Value '{valueString}' is not compatible with expected type - {expectedTypeName}");
            }
        }

        private void ValidateArgument<incomeT>(incomeT value, string methodName, Dictionary<string, string> modelColumnToPropNamesMap = null)
        {
            var argumentsSchemeData = GetMethodDescriptionFromSchema(methodName)?.ArgsData;

            if (argumentsSchemeData == null)
            {
                throw new ArgumentNullException($"Empty arguments scheme data for method {methodName}.");
            }

            if (argumentsSchemeData is DataFrameData)
            {
                ValidateDataframeData(value, argumentsSchemeData as DataFrameData, modelColumnToPropNamesMap);
            }
            else if (argumentsSchemeData is NdarrayData)
            {
                ValidateNdarrayData<incomeT, ICollection>(value, argumentsSchemeData as NdarrayData);
            }
        }

        private void ValidateDataframeData<incomeT>(incomeT value, DataFrameData dataFrameData, Dictionary<string, string> modelColumnToPropNamesMap = null)
        {
            if (modelColumnToPropNamesMap == null)
            {
                throw new ArgumentNullException($"Map of model column names to request object properties is empty.");
            }

            var columnsCountInSchema = dataFrameData.ColumnsData.Count();
            var actualColumnsCount = value.GetType().GetProperties().Count();

            if (actualColumnsCount > columnsCountInSchema)
            {
                throw new ArgumentException($"Count of request object properties is not equal to properties in schema: expected {actualColumnsCount}, but actual is {columnsCountInSchema}");
            }

            var columnsData = dataFrameData.ColumnsData;

            foreach (var columnData in columnsData)
            {
                string? objPropertyName;
                Type? propertyType;

                try
                {
                    objPropertyName = modelColumnToPropNamesMap[columnData.Name];
                }
                catch (KeyNotFoundException)
                {
                    throw new ArgumentException($"Can't find '{columnData.Name}' key in passed column names map");
                }

                try
                {
                    var property = value.GetType().GetProperty(objPropertyName);

                    if (property == null)
                    {
                        throw new ArgumentException($"Can't find '{objPropertyName}' property in request object, although it exists in schema");
                    }

                    propertyType = property.PropertyType;
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Can't get value property '{objPropertyName}': {e.Message}", e);
                }

                try
                {
                    ValidateValueType(columnData.Dtype, propertyType.Name);
                }
                catch (Exception e)
                {
                    throw new ArgumentException($"Invalid argument '{columnData.Name}': {e.Message}");
                }
            }
        }

        private void ValidateNdarrayData<incomeT, ArrayType>(incomeT value, NdarrayData ndArrayData) where ArrayType : class
        {
            var requestArray = value as ArrayType;

            var listElementsQueue = new Queue<Tuple<object, int>>();
            listElementsQueue.Enqueue(Tuple.Create<object, int>(requestArray, 0));

            while (listElementsQueue.Count > 0)
            {
                var currentListElement = listElementsQueue.Dequeue();

                if (currentListElement.Item1 == null)
                {
                    throw new NoNullAllowedException($"There is a null value in ndarray.");
                }

                if (currentListElement.Item1 is ICollection)
                {
                    long? expectedArrayLength;
                    try
                    {
                        expectedArrayLength = ndArrayData.Shape.ElementAt(currentListElement.Item2);
                    }
                    catch (Exception)
                    {
                        throw new ArgumentException($"Unexpected level of nesting in response data - appeared {currentListElement.Item2}, but {ndArrayData.Shape.Count() - 1} is expected as maximum");
                    }

                    var currentArray = currentListElement.Item1 as ICollection;

                    if (expectedArrayLength != null && currentArray.Count != expectedArrayLength)
                    {
                        throw new ArgumentException($"Array {currentArray} does not have expected length - actual is {currentArray.Count}, but {expectedArrayLength} expected");
                    }

                    foreach (var subElement in currentArray)
                    {
                        listElementsQueue.Enqueue(Tuple.Create<object, int>(subElement, currentListElement.Item2 + 1));
                    }
                }
                else
                {
                    if (currentListElement.Item2 != ndArrayData.Shape.Count())
                    {
                        throw new ArgumentException($"Primitive values on nesting level {currentListElement.Item2} appeared, but expected on {ndArrayData.Shape.Count()} level only");
                    }
                    if (ndArrayData?.Dtype != null)
                    {
                        ValidateStringifiedValue(currentListElement.Item1, ndArrayData.Dtype);
                    }
                }
            }
        }

        private void ValidateValueType(string typeNameFromSchema, string actualTypeName)
        {
            string expectedTypeName = "";
            bool unknownType = false;

            try
            {
                expectedTypeName = this.typesMap[typeNameFromSchema];
            }
            catch (KeyNotFoundException)
            {
                unknownType = true;
            }

            if (expectedTypeName != actualTypeName)
            {
                var expectedTypeString = unknownType ? $"equivalent of {actualTypeName}" : expectedTypeName;
                throw new ArgumentException($"incorrect type - current is {actualTypeName}, but {expectedTypeString} expected");
            }
        }

        private MethodDescription GetMethodDescriptionFromSchema(string methodName)
        {
            return this._apiDescription.Methods
                .First(methodDescription => methodDescription.MethodName == methodName);
        }
    }
}
