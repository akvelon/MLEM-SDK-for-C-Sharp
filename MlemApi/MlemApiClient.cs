using System.Text;
using System.Text.Json;
using System.Net.Mime;
using Microsoft.Extensions.Logging;
using MlemApi.Dto;
using MlemApi.Serializing;
using MlemApi.Validation;
using MlemApi.Parsing;
using MlemApi.Logging;
using MlemApi.MessageResources;
using System.ComponentModel;
using System.Globalization;
using MlemApi.Utils;
using Semver;
using MlemApi.Validation.Exceptions;

namespace MlemApi
{
    /// <summary>
    /// Client to access to mlem API
    /// </summary>
    public class MlemApiClient
    {
        private const string PREDICT_METHOD = "predict";

        private readonly HttpClient _httpClient;
        private readonly ILogger? _logger;
        private readonly IValidator? _validator;
        private readonly RequestBuilder _requestBuilder;
        private readonly ApiDescription _apiDescription;
        private readonly DescriptionParser _descriptionParser;

        /// <summary>
        /// If true - turns arguments validation on
        /// </summary>
        public bool ArgumentsValidationIsOn { get; set; }
        /// <summary>
        /// If true - turns response validation on
        /// </summary>
        public bool ResponseValidationIsOn { get; set; } = false;

        /// <summary>
        /// Constructs mlem client
        /// </summary>
        /// <param name="url">url of the deployed mlem model</param>
        /// <param name="logger">logger to be used by mlem client</param>
        /// <param name="httpClient">http client used to send requests to mlem model</param>
        /// <param name="requestSerializer">request serializer used to serialize request to model</param>
        /// <param name="validator">validator - to provide validation for request data, method name and response</param>
        /// <param name="argumentTypesValidationIsOn">if true - turns arguments validation on</param>
        /// <param name="throwErrorIfUnsupportedSchemaVersion">if true - throw the exception if mlem server schema version is different than supported schema version, if false - we just write warning message into the console and continue work</param>
        public MlemApiClient(string url, ILogger<MlemApiClient>? logger = null, HttpClient? httpClient = null,
            IRequestValuesSerializer? requestSerializer = null, IValidator? validator = null, bool argumentTypesValidationIsOn = false, bool throwErrorIfUnsupportedSchemaVersion = true)
        {
            _httpClient = httpClient ?? new HttpClient();
            _logger = logger ?? new DefaultLogger();
            
            _httpClient.BaseAddress = new Uri(url);

            _requestBuilder = new RequestBuilder(requestSerializer ?? new DefaultRequestValueSerializer());

            _descriptionParser = new DescriptionParser(logger);

            _apiDescription = GetDescription();

            CheckSupportedSchemaVersion(_apiDescription, throwErrorIfUnsupportedSchemaVersion);

            _validator = validator ?? new Validator(_apiDescription, null, _logger);

            ArgumentsValidationIsOn = argumentTypesValidationIsOn;
        }

        /// <summary>
        /// Call predict API method
        /// </summary>
        /// <typeparam name="incomeT"></typeparam>
        /// <typeparam name="outcomeT"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<ResultType?> PredictAsync<ResultType, RequestType>(RequestType value, Dictionary<string, string>? modelColumnNamesMap = null)
        {
            return await CallAsync<ResultType, RequestType>(PREDICT_METHOD, new List<RequestType> { value }, modelColumnNamesMap);
        }

        /// <summary>
        /// Call predict API method
        /// </summary>
        /// <typeparam name="incomeT"></typeparam>
        /// <typeparam name="outcomeT"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<ResultType?> PredictAsync<ResultType, RequestType>(IEnumerable<RequestType> values, Dictionary<string, string>? modelColumnNamesMap = null)
        {
            return await CallAsync<ResultType, RequestType>(PREDICT_METHOD, values, modelColumnNamesMap);
        }

        /// <summary>
        /// Call methodName API method
        /// </summary>
        /// <typeparam name="incomeT"></typeparam>
        /// <typeparam name="outcomeT"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="values"></param>
        /// <returns>Response data from mlem model</returns>
        public async Task<ResultType?> CallAsync<ResultType, RequestType>(string methodName, RequestType value, Dictionary<string, string>? modelColumnNamesMap = null)
        {
            return await CallAsync<ResultType, RequestType>(methodName, new List<RequestType> { value }, modelColumnNamesMap);
        }

        /// <summary>
        /// Call methodName API method
        /// </summary>
        /// <typeparam name="ResultType">Type of data being returned from model</typeparam>
        /// <typeparam name="RequestType">Type of input data</typeparam>
        /// <param name="methodName">Method name (like "predict")</param>
        /// <param name="values">Data values</param>
        /// <param name="modelColumnNamesMap">Map from model column names to field names of the RequestType - used for validation only</param>
        /// <returns>Response data from mlem model</returns>
        public async Task<ResultType?> CallAsync<ResultType, RequestType>(string methodName, IEnumerable<RequestType> values, Dictionary<string, string>? modelColumnNamesMap = null)
        {
            _validator?.ValidateMethod(methodName);

            MethodDescription methodDescription = _apiDescription.Methods.First(m => m.MethodName == methodName);

            _validator?.ValidateValues(values, methodName, ArgumentsValidationIsOn, modelColumnNamesMap);

            string argsName = methodDescription.ArgsName;

            var jsonRequest = _requestBuilder.BuildRequest(argsName, values, methodDescription.ArgsData.GetType());

            return await SendPostRequestAsync<ResultType?>(methodName, jsonRequest);
        }

        /// <summary>
        /// Returns api schema desciption retrieved from deployed mlem model
        /// </summary>
        /// <returns>api schema description for deployed mlem model</returns>
        public ApiDescription GetDescription()
        {
            _logger?.LogInformation(string.Format(LM.LogRequestCommand, "interface.json"));

            try
            {
                string response = _httpClient.GetStringAsync("interface.json").Result;

                return _descriptionParser.GetApiDescription(response);
            }
            catch (Exception ex)
            {
                _logger?.LogError(EM.ExceptionGettingApiDescription, ex);

                throw;
            }
        }

        private void CheckSupportedSchemaVersion(ApiDescription apiDescription, bool throwErrorIfUnsupportedSchemaVersion)
        {
            var mlemSchemaSemVersion = SemVersion.Parse(apiDescription.SchemaVersion, SemVersionStyles.Strict);
            var supportedSchemaSemVersion = SemVersion.Parse(ApplicationRestriction.SupportedSchemaVersion, SemVersionStyles.Strict);

            if ((mlemSchemaSemVersion.Major == 0 && mlemSchemaSemVersion.ComparePrecedenceTo(supportedSchemaSemVersion) != 0)
                || mlemSchemaSemVersion.Major != supportedSchemaSemVersion.Major)
            {
                if (throwErrorIfUnsupportedSchemaVersion)
                    throw new InvalidApiSchemaException($"ERROR! Current target mlem schema version - {ApplicationRestriction.SupportedSchemaVersion} is not backward-compatible with mlem server schema version - {_apiDescription.SchemaVersion}.");
                else
                    _logger?.LogWarning($"WARNING! Current target mlem schema version - {ApplicationRestriction.SupportedSchemaVersion} is not backward-compatible with mlem server schema version - {_apiDescription.SchemaVersion}. This may cause compatibility issues and unexpected results.");
            }
        }

        private async Task<T?> SendPostRequestAsync<T>(string command, string requestJsonString)
        {
            _logger?.LogInformation(string.Format(LM.LogRequestCommand, command));
            _logger?.LogInformation(string.Format(LM.LogRequestJson,requestJsonString));

            try
            {
                HttpResponseMessage httpResponse = await _httpClient.PostAsync(
                    command,
                    new StringContent(requestJsonString, Encoding.UTF8, MediaTypeNames.Application.Json));

                _logger?.LogInformation(string.Format(LM.LogResponseStatus, httpResponse.StatusCode));

                string response = await httpResponse.Content.ReadAsStringAsync();

                // MLEM server sends 500 Internal Server Error for any exception case
                // so handle only 200 OK responses
                if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpRequestException(response, null, httpResponse.StatusCode);
                }

                if (ResponseValidationIsOn)
                {
                    _logger?.LogDebug("Response validation is on - started validation");
                    _validator?.ValidateJsonResponse(response, command);
                }

                try
                {
                    T? result = JsonSerializer.Deserialize<T>(response);

                    if (result == null)
                    {
                        _logger?.LogWarning(LM.LogResponseDeserializationNull);
                    }

                    return result;
                }
                catch
                {
                    _logger?.LogInformation($"Response deserialization from json failed - considering responce as plain text");
                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                    
                    return (T) converter.ConvertFromString(null, CultureInfo.InvariantCulture, response);
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, EM.ApiRequestError);

                throw;
            }
        }
    }
}
