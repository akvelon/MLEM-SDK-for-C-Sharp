namespace MlemApi.Validation
{
    /// <summary>
    /// Interface for validator - which provides validation logic for request and response data
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validates method
        /// If method is not supported - throws an error; otherwise finishes successfully
        /// </summary>
        /// <param name="methodName">Method name (like "predict")</param>
        void ValidateMethod(string methodName);
        /// <summary>
        /// Validates values provided as request data
        /// </summary>
        /// <typeparam name="incomeT">Type of the element of the request data list</typeparam>
        /// <param name="values">List of the request data values</param>
        /// <param name="methodName">Method name (like "predict")</param>
        /// <param name="argumentsValidationIsOn">If true - turns on arguments validation </param>
        /// <param name="modelColumnToPropNamesMap">Map from model column names to field names of the RequestType</param>
        void ValidateValues<incomeT>(IEnumerable<incomeT> values,
            string methodName,
            bool argumentsValidationIsOn = true,
            Dictionary<string, string>? modelColumnToPropNamesMap = null
        );
        /// <summary>
        /// Validates response from the model
        /// </summary>
        /// <param name="response">Response from the model (json response)</param>
        /// <param name="methodName">Method name (like "predict")</param>
        void ValidateJsonResponse(string response, string methodName);
    }
}
