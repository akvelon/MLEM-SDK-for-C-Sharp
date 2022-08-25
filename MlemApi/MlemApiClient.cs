using System.Text;
using System.Text.Json;
using System.Net.Mime;
using Microsoft.Extensions.Logging;
using MlemApi.Dto;

namespace MlemApi
{
    /// <summary>
    /// Client to access to mlem API
    /// </summary>
    public class MlemApiClient
    {
        private const string PREDICT_METHOD = "predict";

        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly RequestBuilder _requestBuilder;

        private ApiDescription _apiDescription;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="configuraion"></param>
        public MlemApiClient(string url, ILogger<MlemApiClient> logger = null, HttpClient httpClient = null, IRequestValueSerializer requestSerializer = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            _logger = logger;

            _httpClient.BaseAddress = new Uri(url);

            _requestBuilder = new RequestBuilder(requestSerializer ?? new DefaultRequestValueSerializer());

            _apiDescription = GetDescription();
        }

        /// <summary>
        /// Call predict API method
        /// </summary>
        /// <typeparam name="incomeT"></typeparam>
        /// <typeparam name="outcomeT"></typeparam>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<outcomeT> PredictAsync<incomeT, outcomeT>(IEnumerable<incomeT> values)
        {
            return await CallAsync<incomeT, outcomeT>(PREDICT_METHOD, values);
        }

        /// <summary>
        /// Call methodName API method
        /// </summary>
        /// <typeparam name="incomeT"></typeparam>
        /// <typeparam name="outcomeT"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public async Task<outcomeT> CallAsync<incomeT, outcomeT>(string methodName, IEnumerable<incomeT> values)
        {
            return await DoMlemRequest<incomeT, outcomeT>(methodName, values);
        }

        private async Task<outcomeT> DoMlemRequest<incomeT, outcomeT>(string methodName, IEnumerable<incomeT> values)
        {
            ValidateMethod(methodName);

            ValidateValues(values);

            var argsName = _apiDescription.Methods.First(m => m.MethodName == methodName).ArgsName;

            var jsonRequest = _requestBuilder.BuildRequest(argsName, values);

            return await DoPostRequest<outcomeT>(methodName, jsonRequest);
        }

        private async Task<T> DoPostRequest<T>(string command, string requestJsonString)
        {

            _logger?.LogInformation($"Request command: {command}");

            string responseMessage;
            try
            {
                var response = await _httpClient.PostAsync(
                    command,
                    new StringContent(requestJsonString, Encoding.UTF8, MediaTypeNames.Application.Json));

                _logger?.LogInformation($"Response status: {response.StatusCode}.");

                responseMessage = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpRequestException(responseMessage, null, response.StatusCode);
                }

                try
                {
                    var result = JsonSerializer.Deserialize<T>(responseMessage);

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

        private ApiDescription GetDescription()
        {
            _logger?.LogInformation("Request command: interface.json");

            var requestTask = _httpClient.GetStringAsync("interface.json");
            requestTask.Wait();

            var response = requestTask.Result;

            return DescriptionParser.GetApiDescription(response);
        }

        private void ValidateMethod(string methodName)
        {
            if (!_apiDescription.Methods.Any(m => m.MethodName == methodName))
            {
                var message = $"No method {methodName} in API.";

                _logger?.LogError(message);

                throw new InvalidOperationException(message);
            }
        }

        private void ValidateValues<incomeT>(IEnumerable<incomeT> values)
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
        }
    }
}
