namespace MlemApi.Validation
{
    public interface IValidator
    {
        void ValidateMethod(string methodName);
        void ValidateValues<incomeT>(IEnumerable<incomeT> values,
            string methodName,
            bool argumentTypesValidationIsOn = true,
            Dictionary<string, string>? modelColumnToPropNamesMap = null
        );
        void ValidateJsonResponse(string response, string methodName);
    }
}
