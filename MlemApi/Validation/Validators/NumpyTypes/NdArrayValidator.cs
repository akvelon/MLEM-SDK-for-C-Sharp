using System.Collections;
using System.Data;
using MlemApi.Dto;
using MlemApi.Utils;
using MlemApi.Validation.Exceptions;

namespace MlemApi.Validation.Validators.NumpyTypes
{
    internal class NdArrayValidator : ITypeValidator
    {
        private readonly IPrimitiveTypeHelper primitiveTypeHelper = new PrimitiveTypeHelper();

        public List<Type> SupportedApiDescriptionDataStructure()
        {
            return new List<Type>() { typeof(NdarrayData) };
        }

        public void Validate<T, D>(T value, IApiDescriptionDataStructure apiDescriptionDataStructure, ValidationParameters validationParameters = null, D? additionalData = null, ITypeValidator ? childTypeVallidator = null)
            where D : class?
        {
            NdarrayData ndArrayData = apiDescriptionDataStructure as NdarrayData;
            var requestArray = value as ICollection;

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
                        primitiveTypeHelper.ValidateType(currentListElement.Item1, ndArrayData?.Dtype, validationParameters.ShouldValueBeParsed);
                    }
                }
            }
        }
    }
}
