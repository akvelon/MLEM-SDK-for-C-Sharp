using System.Collections;
using System.Data;
using Microsoft.Extensions.Logging;
using MlemApi.Dto;
using MlemApi.Dto.DataFrameData;
using MlemApi.Validation.Exceptions;
using MlemApi.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MlemApi.MessageResources;

namespace MlemApi.Validation
{
    /// <summary>
    /// Provides validation logic for request or response data
    /// </summary>
    internal class Validator : IValidator
    {
        private readonly ILogger? _logger;
        private readonly ApiDescription _apiDescription;
        private readonly IPrimitiveTypeHelper _primitiveTypeHelper;

        public Validator(ApiDescription _apiDescription, IPrimitiveTypeHelper? primitiveTypeHelper = null, ILogger? logger = null)
        {
            _logger = logger;
            this._apiDescription = _apiDescription;
            this._primitiveTypeHelper = primitiveTypeHelper ?? new PrimitiveTypeHelper();
        }

        public void ValidateMethod(string methodName)
        {
            if (methodName == null)
            {
                throw new ArgumentNullException("Method name should not be null");
            }

            _logger?.LogDebug($"Validating method {methodName}.");

            if (!_apiDescription.Methods.Any(m => m.MethodName == methodName))
            {
                var message = $"No method {methodName} in API.";

                _logger?.LogError(message);

                throw new IllegalPathException(message);
            }
        }

        public void ValidateValues<incomeT>(IEnumerable<incomeT> values,
            string methodName,
            bool argumentTypesValidationIsOn = true,
            Dictionary<string, string>? modelColumnToPropNamesMap = null
        )
        {
            _logger?.LogDebug($"Validating values for method {methodName}.");

            if (values == null)
            {
                throw new ArgumentNullException(message: $"Input value is null: {nameof(values)}.", innerException: null);
            }

            if (!values.Any())
            {
                throw new ArgumentException(string.Format(EM.EmptyArgument, nameof(values)));
            }

            if (argumentTypesValidationIsOn)
            {
                _logger?.LogDebug($"Validation of method's arguments is turned on - proceeding on it.");

                foreach (var valueTuple in values.Select((value, index) => new { index, value }))
                {
                    if (valueTuple.value == null)
                    {
                        throw new ArgumentNullException(message: $"Input value by index {valueTuple.index} is null.", innerException: null);
                    }
                    ValidateArgument(valueTuple.value, methodName, modelColumnToPropNamesMap);
                }
            }
        }

        public void ValidateJsonResponse(string response, string methodName)
        {
            _logger?.LogDebug($"Validating response for method {methodName}.");
            _logger?.LogDebug($"Response content: {response}");

            NdarrayData? methodReturnDataSchema = GetMethodDescriptionFromSchema(methodName)?.ReturnData as NdarrayData;

            if (methodReturnDataSchema == null)
            {
                throw new InvalidApiSchemaException($"Return object type for method {methodName} is empty");
            }

            try
            {
                _logger?.LogDebug($"Trying to parse response as json...");
                JArray parsedResponse = JArray.Parse(response);

                // Objects queue with nesting level
                Queue<Tuple<object, int>> listElementsQueue = new();
                listElementsQueue.Enqueue(Tuple.Create<object, int>(parsedResponse, 0));

                _logger?.LogDebug($"Considering that response should have ndarray type");
                ValidateNdarrayData<JArray, JArray>(parsedResponse, methodReturnDataSchema);
            }
            catch (JsonReaderException e)
            {
                throw new JsonReaderException($"Invalid json response from model - {e.Message}");
            }
        }

        private void ValidateStringifiedValue(object jsonValue, string expectedNumPyTypeName)
        {
            var valueString = jsonValue.ToString();

            _primitiveTypeHelper.ValidateType(valueString, expectedNumPyTypeName);
        }

        private void ValidateArgument<incomeT>(incomeT value, string methodName, Dictionary<string, string>? modelColumnToPropNamesMap = null)
        {
            _logger?.LogDebug($"Validate argument value - {value}");
            var argsData = GetMethodDescriptionFromSchema(methodName)?.ArgsData;
            switch (argsData)
            {
                case DataFrameData dataFrame:
                    {
                        ValidateDataframeData(value, dataFrame, modelColumnToPropNamesMap);
                        break;
                    }
                case NdarrayData ndarrayData:
                    {
                        ValidateNdarrayData<incomeT, ICollection>(value, ndarrayData);
                        break;
                    }
                case null:
                    {
                        throw new InvalidApiSchemaException($"Empty arguments scheme data for method {methodName}.");
                    }
                default:
                    {
                        throw new NotSupportedTypeException($"Not supported argument type for method {methodName}: {argsData.GetType()}.");
                    }
            }
        }

        private void ValidateDataframeData<incomeT>(incomeT value, DataFrameData dataFrameData, Dictionary<string, string>? modelColumnToPropNamesMap = null)
        {
            _logger?.LogDebug($"Validating as dataframe...");
            if (modelColumnToPropNamesMap == null)
            {
                throw new ArgumentNullException($"Map of model column names to request object properties is empty.");
            }

            var columnsCountInSchema = dataFrameData.ColumnsData.Count();
            var actualColumnsCount = value.GetType().GetProperties().Count();

            if (actualColumnsCount > columnsCountInSchema)
            {
                throw new IllegalColumnsNumberException($"Count of request object properties is not equal to properties in schema: expected {columnsCountInSchema}, but actual is {actualColumnsCount}");
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
                    throw new KeyNotFoundException($"Can't find '{columnData.Name}' key in passed column names map");
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
                    throw new KeyNotFoundException($"Can't find '{objPropertyName}' property in request object, although it exists in schema");
                }

                ValidateValueType(columnData.Dtype, propertyType.Name);
            }
        }

        private void ValidateNdarrayData<incomeT, ArrayType>(incomeT value, NdarrayData ndArrayData) where ArrayType : class
        {
            _logger?.LogDebug($"Validating as ndarray...");
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
                        expectedArrayLength = ndArrayData.Shape.ElementAt(currentListElement.Item2) ;
                    }
                    catch (Exception)
                    {
                        throw new IllegalArrayNestingLevelException($"Unexpected level of nesting in response data - appeared {currentListElement.Item2}, but {ndArrayData.Shape.Count() - 1} is expected as maximum");
                    }

                    var currentArray = currentListElement.Item1 as ICollection;

                    if (expectedArrayLength != null && currentArray.Count != expectedArrayLength)
                    {
                        throw new IllegalArrayLengthException($"Array {currentArray} does not have expected length - actual is {currentArray.Count}, but {expectedArrayLength} expected");
                    }

                    foreach (var subElement in currentArray)
                    {
                        listElementsQueue.Enqueue(Tuple.Create(subElement, currentListElement.Item2 + 1));
                    }
                }
                else
                {
                    if (currentListElement.Item2 != ndArrayData.Shape.Count())
                    {
                        throw new IllegalArrayNestingLevelException($"Primitive values on nesting level {currentListElement.Item2} appeared, but expected on {ndArrayData.Shape.Count()} level only");
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
                expectedTypeName = _primitiveTypeHelper.GetMappedDtype(typeNameFromSchema);
            }
            catch (KeyNotFoundException)
            {
                unknownType = true;
            }

            if (expectedTypeName != actualTypeName)
            {
                string expectedTypeString = unknownType ? $"equivalent of {actualTypeName}" : expectedTypeName;
                throw new InvalidTypeException($"incorrect type - current is {actualTypeName}, but {expectedTypeString} expected");
            }
        }

        private MethodDescription GetMethodDescriptionFromSchema(string methodName)
            => _apiDescription.Methods.First(methodDescription => methodDescription.MethodName == methodName);
    }
}
