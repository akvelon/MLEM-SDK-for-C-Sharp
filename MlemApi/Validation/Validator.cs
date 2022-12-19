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
using System.Globalization;

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
                var message = string.Format(EM.NoMethodInApi, methodName);

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
                throw new InvalidApiSchemaException(string.Format(EM.ReturnObjectTypeForMethodIsEmpty, methodName));
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
                throw new JsonReaderException(string.Format(EM.InvalidJsonResponseFromModel, e.Message));
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
                        throw new InvalidApiSchemaException(string.Format(EM.EmptyArgumentsSchemeDataForMethod, methodName));
                    }
                default:
                    {
                        throw new NotSupportedTypeException(string.Format(EM.NotSupportedArgumentType, methodName, argsData.GetType()));
                    }
            }
        }

        private void ValidateDataframeData<incomeT>(incomeT value, DataFrameData dataFrameData, Dictionary<string, string>? modelColumnToPropNamesMap = null)
        {
            _logger?.LogDebug($"Validating as dataframe...");
            if (modelColumnToPropNamesMap == null)
            {
                throw new ArgumentNullException(EM.MapModelColumnsIsEmpty);
            }

            var columnsCountInSchema = dataFrameData.ColumnsData.Count();
            var actualColumnsCount = value.GetType().GetProperties().Count();

            if (actualColumnsCount > columnsCountInSchema)
            {
                throw new IllegalColumnsNumberException(string.Format(EM.NotEqualCountOfRequestObjectProperties, columnsCountInSchema, actualColumnsCount));
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
                    throw new KeyNotFoundException(string.Format(EM.CantFindColumnKeyInMap, columnData.Name));
                }

                try
                {
                    var property = value.GetType().GetProperty(objPropertyName);

                    if (property == null)
                    {
                        throw new ArgumentException(string.Format(EM.CantFindPropertyInObject, objPropertyName));
                    }

                    propertyType = property.PropertyType;
                }
                catch (Exception e)
                {
                    throw new KeyNotFoundException(string.Format(EM.CantFindPropertyInObject, objPropertyName));
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
                    throw new NoNullAllowedException(EM.NullValueInNdArray);
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
                        throw new IllegalArrayNestingLevelException(string.Format(EM.UnexpectedLevelOfNestingResponseData, currentListElement.Item2, ndArrayData.Shape.Count() - 1));
                    }

                    var currentArray = currentListElement.Item1 as ICollection;

                    if (expectedArrayLength != null && currentArray.Count != expectedArrayLength)
                    {
                        throw new IllegalArrayLengthException(string.Format(EM.ArrayUnexpectedLength, currentArray, currentArray.Count, expectedArrayLength));
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
                        throw new IllegalArrayNestingLevelException(string.Format(EM.PrimitiveValueUnexpectedLevel, currentListElement.Item2, ndArrayData.Shape.Count()));
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
                if (unknownType)
                {
                    throw new InvalidTypeException(string.Format(EM.IncorrectValueTypeEquivalent, actualTypeName, actualTypeName));
                }
                else
                {
                    throw new InvalidTypeException(string.Format(EM.IncorrectValueType, actualTypeName, expectedTypeName));
                }
            }
        }

        private MethodDescription GetMethodDescriptionFromSchema(string methodName)
            => _apiDescription.Methods.First(methodDescription => methodDescription.MethodName == methodName);
    }
}
