using MlemApi.Dto;

namespace MlemApi.Validation.Validators
{
    internal interface ITypeValidator
    {
        /// <summary>
        /// Returns types of api schema data structure supported for validation (inherited from IApiDescriptionDataStructure)
        /// </summary>
        /// <returns>List of types of api schema data structure supported for validation</returns>
        List<Type> SupportedApiDescriptionDataStructure();
        /// <summary>
        /// Validates value by provided api schema structure and additional data
        /// If value is invalid - throws an error, otherwise - completes successfully
        /// </summary>
        /// <param name="value">Value to be validated</param>
        /// <param name="apiDescriptionDataStructure">Api schema structure to be used for validation</param>
        /// <param name="validationParameters">Common validation parameters for type validators</param>
        /// <param name="additionalData">Additional data for validation</param>
        /// <param name="childTypeVallidator">Type validator for child values</param>
        void Validate<T, D>(T value, IApiDescriptionDataStructure apiDescriptionDataStructure, ValidationParameters validationParameters = null, D? additionalData = null, ITypeValidator ? childTypeVallidator = null)
             where D : class?;
    }
}
