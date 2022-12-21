using System.Data;
using MlemApi.Dto;
using MlemApi.Utils;

namespace MlemApi.Validation.Validators.PrimitiveTypes
{
    internal class PrimitiveTypeValidator : ITypeValidator
    {
        private readonly IPrimitiveTypeHelper primitiveTypeHelper = new PrimitiveTypeHelper();

        public List<Type> SupportedApiDescriptionDataStructure()
        {
            return new List<Type>() { typeof(PrimitiveData) };
        }

        public void Validate<T, D>(T value, IApiDescriptionDataStructure apiDescriptionDataStructure, ValidationParameters validationParameters = null, D? additionalData = null, ITypeValidator? childTypeVallidator = null)
            where D : class?
        {
            var primitiveData = apiDescriptionDataStructure as PrimitiveData;

            if (value == null)
            {
                throw new NoNullAllowedException($"Null primitive value not allowed.");
            }

            primitiveTypeHelper.ValidateType(value, primitiveData.Ptype, validationParameters.ShouldValueBeParsed);
        }
    }
}
