using System.Collections;
using MlemApi.Dto;

namespace MlemApi.Validation.Validators.PrimitiveTypes
{
    internal class ArrayTypeValidator : ITypeValidator
    {
        public List<Type> SupportedApiDescriptionDataStructure()
        {
            return new List<Type>() { typeof(ListData) };
        }

        public void Validate<T, D>(T value, IApiDescriptionDataStructure apiDescriptionDataStructure, ValidationParameters validationParameters = null, D? additionalData = null, ITypeValidator? childTypeVallidator = null)
             where D : class?
        {
            var arrayData = apiDescriptionDataStructure as ListData;
            var arrayValue = value as ICollection;

            var arrayEnumerator = arrayValue.GetEnumerator();
            arrayEnumerator.MoveNext();
            var firstElement = arrayEnumerator.Current;

            childTypeVallidator.Validate(firstElement, arrayData.Items.First(), validationParameters, additionalData, childTypeVallidator);
        }
    }
}
