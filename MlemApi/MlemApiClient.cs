using System.Text;
using System.Text.Json;
using System.Net.Mime;
using Microsoft.Extensions.Logging;
using MlemApi.Dto;
using MlemApi.Serializing;
using MlemApi.Dto.DataFrameArgumentData;
using MlemApi.Validation.Exceptions;
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
        public async Task<ResultType?> PredictAsync<ResultType, RequestType>(RequestType value, Dictionary<string, string> modelColumnNamesMap = null)
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
        public async Task<ResultType?> PredictAsync<ResultType, RequestType>(IEnumerable<RequestType> values, Dictionary<string, string > modelColumnNamesMap = null)
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
        public async Task<ResultType?> CallAsync<ResultType, RequestType>(string methodName, RequestType value, Dictionary<string, string> modelColumnNamesMap = null)
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
        public async Task<ResultType?> CallAsync<ResultType, RequestType>(string methodName, IEnumerable<RequestType> values, Dictionary<string, string> modelColumnNamesMap = null)
        {
            this._validator?.ValidateMethod(methodName);

            var methodDescription = _apiDescription.Methods.First(m => m.MethodName == methodName);

            var requestObjectType = GetMethodArgumentType(methodDescription);

            this._validator?.ValidateValues(values, methodName, ArgumentTypesValidationIsOn, modelColumnNamesMap);

            var argsName = methodDescription.ArgsName;

            var jsonRequest = _requestBuilder.BuildRequest(argsName, values, requestObjectType);

            return await SendPostRequestAsync<ResultType?>(methodName, jsonRequest);
        }

        internal ApiDescription GetDescription()
        {
            _logger?.LogInformation("Request command: interface.json");

            try
            {
                var requestTask = _httpClient.GetStringAsync("interface.json");
                requestTask.Wait();

                var response = requestTask.Result;

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
                var data = "{\"data\": [[\r\n  0.22390437670507946,\r\n  0.8755668372255736,\r\n  0.5611885525405516,\r\n  0.32394306840890486,\r\n  0.0037118855705782217,\r\n  0.8780319892030065,\r\n  0.8765942138523862,\r\n  0.5690406704495534,\r\n  0.7633158306299445,\r\n  0.9936924225477365,\r\n  0.8052452936065757,\r\n  0.8674048663570549,\r\n  0.36449478966506377,\r\n  0.43714033209832537,\r\n  0.76142034623535,\r\n  0.5076381413926438,\r\n  0.7117385505272897,\r\n  0.03426174895551781,\r\n  0.05294998110404214,\r\n  0.4791296700117913,\r\n  0.9594266734630614,\r\n  0.7237727004230323,\r\n  0.04152250039690264,\r\n  0.8545869349843117,\r\n  0.5222667836626967,\r\n  0.035466114538043714,\r\n  0.34005752628167374,\r\n  0.5354634106167977,\r\n  0.8544202976048005,\r\n  0.03169251980043997,\r\n  0.8837447950387551,\r\n  0.07318319533788942,\r\n  0.16770210480527326,\r\n  0.28045857818610653,\r\n  0.8969163167685288,\r\n  0.2986557773648748,\r\n  0.2948062081064321,\r\n  0.5701379125651316,\r\n  0.9579629448802064,\r\n  0.8484550664794912,\r\n  0.5091959425006576,\r\n  0.6761147501694055,\r\n  0.3670418492517107,\r\n  0.09311250068597332,\r\n  0.5023206438070372,\r\n  0.1767084366441366,\r\n  0.2093504190599792,\r\n  0.9011654949888794,\r\n  0.18990477764790237,\r\n  0.20627785389101394,\r\n  0.4198071683949789,\r\n  0.5118977041804791,\r\n  0.9658676743026027,\r\n  0.7345787672534155,\r\n  0.41780219603940716,\r\n  0.37697144330469723,\r\n  0.531249715207828,\r\n  0.27821854959331915,\r\n  0.4515378027693925,\r\n  0.5705903799002224,\r\n  0.4751969061453537,\r\n  0.3534651623527718,\r\n  0.10706533298844323,\r\n  0.37436080356877866\r\n]]}";
                HttpResponseMessage httpResponse = await _httpClient.PostAsync(
                    command,
                    new StringContent(data, Encoding.UTF8, MediaTypeNames.Application.Json));
               
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
                    this._validator?.ValidateJsonResponse(response, command);
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
            finally
            {
                var a = 4;
                a = a;
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
