using System.Data;
using Microsoft.Extensions.Logging;
using MlemApi.Dto;
using MlemApi.Validation.Exceptions;
using MlemApi.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MlemApi.MessageResources;
using MlemApi.Validation.Validators;

namespace MlemApi.Validation
{
    /// <summary>
    /// Provides validation logic for request or response data
    /// </summary>
    internal class Validator : IValidator
    {
        private readonly ILogger? _logger;
        private readonly ApiDescription _apiDescription;
        private readonly IPrimitiveTypeHelper _primitiveTypeHelper;
        private readonly ITypeValidator _rootTypeValidator;

        public Validator(ApiDescription _apiDescription, IPrimitiveTypeHelper? primitiveTypeHelper = null, ILogger? logger = null, RootTypeValidator rootTypeValidator = null)
        {
            _logger = logger;
            this._apiDescription = _apiDescription;
            this._primitiveTypeHelper = primitiveTypeHelper ?? new PrimitiveTypeHelper();
            this._rootTypeValidator = rootTypeValidator ?? new RootTypeValidator();
        }

        public void ValidateMethod(string methodName)
        {
            if (methodName == null)
            {
                throw new ArgumentNullException("Method name should not be null");
            }

            _logger?.LogDebug($"Validating method {methodName}.");

            if (!_apiDescription.Methods.Any(m => m.MethodName == methodName))
            {
                var message = string.Format(EM.NoMethodInApi, methodName);

                _logger?.LogError(message);

                throw new IllegalPathException(message);
            }
        }

        public void ValidateValues<incomeT>(IEnumerable<incomeT> values,
            string methodName,
            bool argumentTypesValidationIsOn = true,
            Dictionary<string, string>? modelColumnToPropNamesMap = null
        )
        {
            _logger?.LogDebug($"Validating values for method {methodName}.");

            if (values == null)
            {
                throw new ArgumentNullException(message: $"Input value is null: {nameof(values)}.", innerException: null);
            }

            if (!values.Any())
            {
                throw new ArgumentException(string.Format(EM.EmptyArgument, nameof(values)));
            }

            if (argumentTypesValidationIsOn)
            {
                _logger?.LogDebug($"Validation of method's arguments is turned on - proceeding on it.");

                foreach (var valueTuple in values.Select((value, index) => new { index, value }))
                {
                    if (valueTuple.value == null)
                    {
                        throw new ArgumentNullException(message: $"Input value by index {valueTuple.index} is null.", innerException: null);
                    }
                    ValidateArgument(valueTuple.value, methodName, modelColumnToPropNamesMap);
                }
            }
        }

        public void ValidateJsonResponse(string response, string methodName)
        {
            _logger?.LogDebug($"Validating response for method {methodName}.");
            _logger?.LogDebug($"Response content: {response}");
            var methodReturnDataSchema = GetMethodDescriptionFromSchema(methodName)?.ReturnData;

            if (methodReturnDataSchema == null)
            {
                _logger?.LogInformation($"Return object type for method {methodName} is empty - skipping validation");
                return;
            }

            try
            {
                // by default - use as plain text
                object parsedValue = response;
                try
                {
                    _logger?.LogDebug($"Trying to parse response as json...");
                    parsedValue = JObject.Parse(response);
                }
                catch
                {
                    try
                    {
                        parsedValue = JArray.Parse(response);
                    }
                    catch { }
                }


                _rootTypeValidator.Validate(
                    parsedValue,
                    methodReturnDataSchema,
                    new() { ShouldValueBeParsed = true },
                    _rootTypeValidator
                );
            }
            catch (JsonReaderException e)
            {
                throw new JsonReaderException(string.Format(EM.InvalidJsonResponseFromModel, e.Message));
            }
        }

        private void ValidateArgument<incomeT>(incomeT value, string methodName, Dictionary<string, string>? modelColumnToPropNamesMap = null)
        {
            _logger?.LogDebug($"Validating argument for method {methodName}");
            var argsData = GetMethodDescriptionFromSchema(methodName)?.ArgsData.DataType;
            if (argsData == null)
            {
                throw new InvalidApiSchemaException($"Empty arguments scheme data for method {methodName}.");
            }

            _rootTypeValidator.Validate(
                value,
                argsData,
                new() { ShouldValueBeParsed = false },
                modelColumnToPropNamesMap,
                _rootTypeValidator
            );
        }

        private MethodDescription GetMethodDescriptionFromSchema(string methodName)
            => _apiDescription.Methods.First(methodDescription => methodDescription.MethodName == methodName);
    }
}
