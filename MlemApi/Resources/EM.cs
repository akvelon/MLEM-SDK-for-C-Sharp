// Auto-generated.
// Do NOT change this file manually. To add or change any resource, change the submodule.

namespace MlemApi.MessageResources
{
    /// <summary>
    /// EM (Error Messages) is a collection of string constants, used to define exception messaged.
    /// It's an auto-generated file from `ResourcesGenerator\CommonResources\Error_messages.json` file,
    /// that is the shared string resource between .NET and Java clients and allows to sync exception messaged between them.
    /// Use these constans derictly or with string.Format(...) if a resource string requests arguments.
    /// </summary>
    internal static class EM
    { 
        public const string InvalidApiSchema = "Invalid api schema";
        public const string UknownMethodArgument = "Uknown method arguments data: {0}. 'dataframe' or 'ndarray' are expected";
        public const string NoValidationLogic = "No validation logic for type {0}";
        public const string InvalidType = "Value '{0}' is not compatible with expected type {1}";
        public const string EmptyArgument = "{0} cannot be empty";
        public const string InvalidParametersCount = "Actual parameters number: {0}, expected: {1}";
    }
}