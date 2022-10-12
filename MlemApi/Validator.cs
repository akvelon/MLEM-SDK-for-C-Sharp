using Microsoft.Extensions.Logging;
using MlemApi.Dto;

namespace MlemApi
{
    internal class Validator : IValidator
    {
        private readonly ILogger? _logger;
        private readonly ApiDescription _apiDescription;

        /// <summary>
        /// Map of Numpy types (used in sklearn) to C# primitive types
        /// See https://gist.github.com/robbmcleod/73ca42da5984e6d0e5b6ad28bc4a504efor for the referenced list of types
        /// </summary>
        private readonly Dictionary<string, string> typesMap = new Dictionary<string, string>()
        {
            { "float32", "Single" },
            { "float64", "Double" },
            { "int8", "SByte" },
            { "int16", "Int16" },
            { "int32", "Int32" },
            { "int64", "Int64" },
            { "uint8", "Byte" },
            { "uint16", "UInt16" },
            { "uint32", "UInt32" },
            { "uint64", "UInt64" },
            { "bool", "Boolean" },
        };

        public Validator(ApiDescription _apiDescription, ILogger logger = null)
        {
            this._logger = logger;
            this._apiDescription = _apiDescription;
        }

        public void ValidateMethod(string methodName)
        {
            if (!_apiDescription.Methods.Any(m => m.MethodName == methodName))
            {
                var message = $"No method {methodName} in API.";

                _logger?.LogError(message);

                throw new InvalidOperationException(message);
            }
        }

        public void ValidateValues<incomeT>(IEnumerable<incomeT> values,
            string methodName,
            bool argumentTypesValidationIsOn = true,
            Dictionary<string, string> modelColumnToPropNamesMap = null
        )
        {
            if (values == null)
            {
                _logger?.LogError($"Input value is null: {nameof(values)}.");

                throw new ArgumentNullException(nameof(values));
            }

            if (!values.Any())
            {
                _logger?.LogError($"Input value is empty: {nameof(values)}.");

                throw new ArgumentException($"{nameof(values)} cannot be empty.");
            }

            if (argumentTypesValidationIsOn)
            {
                foreach (var value in values)
                {
                    ValidateValue(value, methodName, modelColumnToPropNamesMap);
                }
            }
        }

        private void ValidateValue<incomeT>(incomeT Value, string methodName, Dictionary<string, string> modelColumnToPropNamesMap = null)
        {
            var argumentsSchemeData = this._apiDescription.Methods
                .First(methodDescription => methodDescription.MethodName == methodName).ArgsData;

            if (modelColumnToPropNamesMap == null)
            {
                throw new ArgumentNullException($"Map of model column names to request object properties is empty.");
            }

            if (argumentsSchemeData == null)
            {
                throw new ArgumentNullException($"Empty arguments scheme data for method {methodName}.");
            }

            foreach (var argumentData in argumentsSchemeData)
            {
                string? objPropertyName;
                Type? propertyType;
                try
                {
                    objPropertyName = modelColumnToPropNamesMap[argumentData.ArgumentName];
                }
                catch (KeyNotFoundException)
                {
                    throw new ArgumentException($"Can't find '{argumentData.ArgumentName}' key in passed column names map");
                }

                try
                {
                    propertyType = Value.GetType().GetProperty(objPropertyName).PropertyType;
                }
                catch (NullReferenceException)
                {
                    throw new ArgumentException($"Can't find '{objPropertyName}' property in request object, although it exists in schema");
                }

                if (!this.typesMap.ContainsKey(argumentData.ArgumentType)) {
                    throw new ArgumentException($"Unknown type of argument - {propertyType.Name}");
                }

                if (this.typesMap[argumentData.ArgumentType] != propertyType.Name)
                {
                    string expectedType;
                    try
                    {
                        expectedType = this.typesMap[argumentData.ArgumentType];
                    }
                    catch (KeyNotFoundException)
                    {
                        expectedType = $"equivalent of {argumentData.ArgumentType}";
                    }

                    throw new ArgumentException($"Incorrect type of argument '{argumentData.ArgumentName} - current is {propertyType.Name}, but {expectedType} expected");
                }
            }
        }
    }
}
