using System.Text.Json;
using Microsoft.Extensions.Logging;
using MlemApi.Dto;
using MlemApi.Serializing;
using MlemApi.Validation;
using MlemApi.Parsing;
using MlemApi.Logging;
using MlemApi.MessageResources;
using System.ComponentModel;
using System.Globalization;

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
        public MlemApiClient(string url, ILogger<MlemApiClient>? logger = null, HttpClient? httpClient = null,
            IRequestValuesSerializer? requestSerializer = null, IValidator? validator = null, bool argumentTypesValidationIsOn = false)
        {
            _httpClient = httpClient ?? new HttpClient();
            _logger = logger ?? new DefaultLogger();
            
            _httpClient.BaseAddress = new Uri(url);

            _requestBuilder = new RequestBuilder(requestSerializer ?? new DefaultRequestValueSerializer());

            _descriptionParser = new DescriptionParser(logger);

            _apiDescription = GetDescription();

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

            var serializedObject = methodDescription.ArgsData.Serializer
                .BuildRequest(argsName, values, methodDescription.ArgsData.DataType.GetType());

            return await SendPostRequestAsync<ResultType?>(methodName, serializedObject, methodDescription.ReturnData.Serializer);
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

        private async Task<T?> SendPostRequestAsync<T>(string command, HttpContent content, IResponseSerializer responseSerializer)
        {
            _logger?.LogInformation(string.Format(LM.LogRequestCommand, command));
            _logger?.LogInformation(string.Format(LM.LogRequestJson, content));

            try
            {
                HttpResponseMessage httpResponse = await _httpClient.PostAsync(command, content);

                _logger?.LogInformation(string.Format(LM.LogResponseStatus, httpResponse.StatusCode));

                string response = await httpResponse.Content.ReadAsStringAsync();

                // MLEM server sends 500 Internal Server Error for any exception case
                // so handle only 200 OK responses
                if (httpResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpRequestException(response, null, httpResponse.StatusCode);
                }

                return responseSerializer.Serialize<T>(httpResponse);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, EM.ApiRequestError);

                throw;
            }
        }
    }
}
