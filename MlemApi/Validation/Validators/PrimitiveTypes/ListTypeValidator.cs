using System.Collections;
using MlemApi.Dto;

namespace MlemApi.Validation.Validators.PrimitiveTypes
{
    internal class ListTypeValidator : ITypeValidator
    {
        public List<Type> SupportedApiDescriptionDataStructure()
        {
            return new List<Type>() { typeof(ListData) };
        }

        public void Validate<T, D>(T value, IApiDescriptionDataStructure apiDescriptionDataStructure, ValidationParameters validationParameters = null, D? additionalData = null, ITypeValidator? childTypeVallidator = null)
             where D : class?
        {
            var listData = apiDescriptionDataStructure as ListData;
            var listValue = value as ICollection;

            var listEnumerator = listValue.GetEnumerator();
            listEnumerator.MoveNext();
            var firstElement = listEnumerator.Current;

            childTypeVallidator.Validate(firstElement, listData.Items.First(), validationParameters, additionalData, childTypeVallidator);
        }
    }
}
