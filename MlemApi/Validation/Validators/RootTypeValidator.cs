using MlemApi.Dto;
using MlemApi.Validation.Exceptions;
using MlemApi.Validation.Validators.NumpyTypes;
using MlemApi.Validation.Validators.PandasTypes;
using MlemApi.Validation.Validators.PrimitiveTypes;

namespace MlemApi.Validation.Validators
{
    internal class RootTypeValidator : ITypeValidator
    {
        private readonly List<ITypeValidator> typeValidators = new List<ITypeValidator> {
            new NdArrayValidator(),
            new DataFrameValidator(),
            new ListTypeValidator(),
            new PrimitiveTypeValidator(),
        };

        public List<Type> SupportedApiDescriptionDataStructure()
        {
            var resList = new List<Type>();

            typeValidators.ForEach(typeValidator =>
            {
                resList.AddRange(typeValidator.SupportedApiDescriptionDataStructure());
            });

            return resList;
        }

        public void Validate<T, D>(T value, IApiDescriptionDataStructure apiDescriptionDataStructure, ValidationParameters validationParameters = null, D? additionalData = null, ITypeValidator? childTypeVallidator = null)
            where D : class?
        {
            var typeValidator = typeValidators.Find(typeValidator =>
            {
                var supportedTypes = typeValidator.SupportedApiDescriptionDataStructure();

                return supportedTypes.FindIndex(type => type == apiDescriptionDataStructure.GetType()) >= 0;
            });

            if (typeValidator == null)
            {
                throw new NotSupportedTypeException($"Not supported type: {apiDescriptionDataStructure.GetType()}.");
            }

            typeValidator?.Validate(value, apiDescriptionDataStructure, validationParameters, additionalData, childTypeVallidator);
        }
    }
}
