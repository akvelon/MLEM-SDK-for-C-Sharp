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
        public const string ApiRequestError = "API request error.";
        public const string ArrayUnexpectedLength = "Array {0} does not have expected length - actual is {1}, but {2} expected";
        public const string CantFindColumnKeyInMap = "Can't find '{0}' key in passed column names map";
        public const string CantFindPropertyInObject = "Can't find '{0}' property in request object, although it exists in schema";
        public const string EmptyArgument = "{0} cannot be empty";
        public const string EmptyArgumentsSchemeDataForMethod = "Empty arguments scheme data for method {0}.";
        public const string ErrorDuringArgsParsing = "Error during args parsing for method {0}: {1}. Args will be considered as empty.";
        public const string ErrorDurngReturnObjectParsing = "Error during return object schema parsing for method {0}: {1}. Return object schema will be considered as empty.";
        public const string ExceptionGettingApiDescription = "Exception of getting API description";
        public const string IncorrectValueType = "Incorrect type - current is {0}, but {1} expected";
        public const string IncorrectValueTypeEquivalent = "Incorrect type - current is {0}, but equivalent of {1} expected";
        public const string InputValueIsEmpty = "Input value is empty: {0}.";
        public const string InputValueIsNull = "Input value is null: {0}.";
        public const string InvalidApiSchema = "Invalid api schema";
        public const string InvalidJsonResponseFromModel = "Invalid json response from model - {0}";
        public const string InvalidParametersCount = "Actual parameters number: {0}, expected: {1}";
        public const string InvalidType = "Value '{0}' is not compatible with expected type {1}";
        public const string MapModelColumnsIsEmpty = "Map of model column names to request object properties is empty";
        public const string NoMethodInApi = "No method {0} in API.";
        public const string NotEqualCountOfRequestObjectProperties = "Count of request object properties is not equal to properties in schema: expected {0}, but actual is {1}";
        public const string NotSupportedArgumentType = "Not supported argument type for method {0}: {1}.";
        public const string NoValidationLogic = "No validation logic for type {0}";
        public const string NullValueInNdArray = "There is a null value in ndarray.";
        public const string PrimitiveValueUnexpectedLevel = "Primitive values on nesting level {0} appeared, but expected on {1} level only";
        public const string ReturnObjectTypeForMethodIsEmpty = "Return object type for method {0} is empty";
        public const string UknownMethodArgument = "Uknown method arguments data: {0}. 'dataframe' or 'ndarray' are expected";
        public const string UnexpectedLevelOfNestingResponseData = "Unexpected level of nesting in response data - appeared {0}, but {1} is expected as maximum";
        public const string UnknownDataTypeInSchema = "Unknown data type in schema: {0}";
        public const string UnknownRequestObjectType = "Unknown request object type: {0}. 'dataframe' or 'ndarray' are expected";
        public const string UnknownValueType = "Unknown value type - {0}";
    }
}
