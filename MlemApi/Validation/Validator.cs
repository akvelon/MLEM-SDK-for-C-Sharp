using System.Collections;
using System.Data;
using Microsoft.Extensions.Logging;
using MlemApi.Dto;
using MlemApi.Dto.DataFrameArgumentData;
using MlemApi.Validation.Exceptions;
using MlemApi.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MlemApi.Validation
{
    internal class Validator : IValidator
    {
        private readonly ILogger? _logger;
        private readonly ApiDescription _apiDescription;
        private readonly IPrimitiveTypeHelper primitiveTypeHelper;

        public Validator(ApiDescription _apiDescription, IPrimitiveTypeHelper primitiveTypeHelper = null, ILogger logger = null)
        {
            _logger = logger;
            this._apiDescription = _apiDescription;
            this.primitiveTypeHelper = primitiveTypeHelper ?? new PrimitiveTypeHelper();
        }

        public void ValidateMethod(string methodName)
        {
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
            var valueString = jsonValue.ToString();

            this.primitiveTypeHelper.ValidateType(valueString, expectedNumPyTypeName);
        }

        private void ValidateArgument<incomeT>(incomeT value, string methodName, Dictionary<string, string> modelColumnToPropNamesMap = null)
        {
            var argumentsSchemeData = GetMethodDescriptionFromSchema(methodName)?.ArgsData;

            if (argumentsSchemeData == null)
            {
                throw new InvalidApiSchemaException($"Empty arguments scheme data for method {methodName}.");
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
                throw new IllegalColumnsNumberException($"Count of request object properties is not equal to properties in schema: expected {actualColumnsCount}, but actual is {columnsCountInSchema}");
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
                        throw new IllegalArrayNestingLevel($"Unexpected level of nesting in response data - appeared {currentListElement.Item2}, but {ndArrayData.Shape.Count() - 1} is expected as maximum");
                    }

                    var currentArray = currentListElement.Item1 as ICollection;

                    if (expectedArrayLength != null && currentArray.Count != expectedArrayLength)
                    {
                        throw new IllegalArrayLength($"Array {currentArray} does not have expected length - actual is {currentArray.Count}, but {expectedArrayLength} expected");
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
                        throw new IllegalArrayNestingLevel($"Primitive values on nesting level {currentListElement.Item2} appeared, but expected on {ndArrayData.Shape.Count()} level only");
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
                expectedTypeName = this.primitiveTypeHelper.GetMappedDtype(typeNameFromSchema);
            }
            catch (KeyNotFoundException)
            {
                unknownType = true;
            }

            if (expectedTypeName != actualTypeName)
            {
                var expectedTypeString = unknownType ? $"equivalent of {actualTypeName}" : expectedTypeName;
                throw new InvalidTypeException($"incorrect type - current is {actualTypeName}, but {expectedTypeString} expected");
            }
        }

        private MethodDescription GetMethodDescriptionFromSchema(string methodName)
        {
            return _apiDescription.Methods
                .First(methodDescription => methodDescription.MethodName == methodName);
        }
    }
}
