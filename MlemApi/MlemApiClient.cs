using System.Text;
using System.Text.Json;
using System.Net.Mime;
using Microsoft.Extensions.Logging;
using MlemApi.Dto;
using MlemApi.Serializing;
using MlemApi.Dto.DataFrameData;
using MlemApi.Validation;

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

        public bool ArgumentTypesValidationIsOn { get; set; }
        public bool ResponseValidationIsOn { get; set; } = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="configuraion"></param>
        public MlemApiClient(string url, ILogger<MlemApiClient>? logger = null, HttpClient? httpClient = null,
            IRequestValuesSerializer? requestSerializer = null, IValidator? validator = null, bool argumentTypesValidationIsOn = true)
        {
            _httpClient = httpClient ?? new HttpClient();
            _logger = logger;

            _httpClient.BaseAddress = new Uri(url);

            _requestBuilder = new RequestBuilder(requestSerializer ?? new DefaultRequestValueSerializer());

            _apiDescription = GetDescription();

            _validator = validator ?? new Validator(_apiDescription, null, _logger);

            ArgumentTypesValidationIsOn = argumentTypesValidationIsOn;
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
        /// <returns></returns>
        public async Task<ResultType?> CallAsync<ResultType, RequestType>(string methodName, RequestType value, Dictionary<string, string>? modelColumnNamesMap = null)
        {
            return await CallAsync<ResultType, RequestType>(methodName, new List<RequestType> { value }, modelColumnNamesMap);
        }

        /// <summary>
        /// Call methodName API method
        /// </summary>
        /// <typeparam name="incomeT"></typeparam>
        /// <typeparam name="outcomeT"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<ResultType?> CallAsync<ResultType, RequestType>(string methodName, IEnumerable<RequestType> values, Dictionary<string, string>? modelColumnNamesMap = null)
        {
            _validator?.ValidateMethod(methodName);

            MethodDescription methodDescription = _apiDescription.Methods.First(m => m.MethodName == methodName);

            string requestObjectType = GetMethodArgumentType(methodDescription);

            _validator?.ValidateValues(values, methodName, ArgumentTypesValidationIsOn, modelColumnNamesMap);

            string argsName = methodDescription.ArgsName;

            string jsonRequest = _requestBuilder.BuildRequest(argsName, values, requestObjectType);

            return await SendPostRequestAsync<ResultType?>(methodName, jsonRequest);
        }

        internal ApiDescription GetDescription()
        {
            _logger?.LogInformation("Request command: interface.json");

            try
            {
                string response = _httpClient.GetStringAsync("interface.json").Result;

                return DescriptionParser.GetApiDescription(response);
            }
            catch (Exception ex)
            {
                _logger?.LogError("Exception of getting API description", ex);

                throw;
            }
        }

        private async Task<T?> SendPostRequestAsync<T>(string command, string requestJsonString)
        {
            _logger?.LogInformation($"Request command: {command}");
            _logger?.LogInformation($"Request JSON string: {requestJsonString}");

            try
            {
                HttpResponseMessage httpResponse = await _httpClient.PostAsync(
                    command,
                    new StringContent(requestJsonString, Encoding.UTF8, MediaTypeNames.Application.Json));

                _logger?.LogInformation($"Response status: {httpResponse.StatusCode}.");

                string response = await httpResponse.Content.ReadAsStringAsync();

                // MLEM server sends 500 Internal Server Error for any exception case
                // so handle only 200 OK responses
                if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpRequestException(response, null, httpResponse.StatusCode);
                }

                if (ResponseValidationIsOn)
                {
                    _validator?.ValidateJsonResponse(response, command);
                }

                try
                {
                    T? result = JsonSerializer.Deserialize<T>(response);

                    if (result == null)
                    {
                        _logger?.LogWarning($"Response deserialization result is null.");
                    }

                    return result;
                }
                catch
                {
                    _logger?.LogError($"Response deserialization error.");

                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"API request error.");

                throw;
            }
        }

        private string GetMethodArgumentType(MethodDescription methodDescription)
            => methodDescription.ArgsData switch
            {
                NdarrayData => "ndarray",
                DataFrameData => "dataframe",
                _ => throw new Exception("Unknown method argument type - dataframe or ndarray is expected")
            };
    }
}
