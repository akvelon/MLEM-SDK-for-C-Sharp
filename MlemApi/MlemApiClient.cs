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
    public class MlemApiClient<incomeT, outcomeT> : IMlemApiClient<incomeT, outcomeT>
    {
        private readonly HttpClient _httpClient;
        private readonly IMlemApiConfiguration _configuraion;
        private readonly ILogger _logger;

        private ApiDescription _apiDescription;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="configuraion"></param>
        public MlemApiClient(HttpClient httpClient, IMlemApiConfiguration configuraion, ILogger<MlemApiClient<incomeT, outcomeT>> logger)
        {
            _httpClient = httpClient;
            _configuraion = configuraion;
            _logger = logger;

            _httpClient.BaseAddress = new Uri(_configuraion.Url);

            _apiDescription = GetDescription();
        }

        private ApiDescription GetDescription()
        {
            var requestTask = _httpClient.GetStringAsync("interface.json");
            requestTask.Wait();

            var response = requestTask.Result;

            return DescriptionParser.GetApiDescription(response);
        }

        /// <summary>
        /// Predict method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<List<outcomeT>> GetPredictAsync(string methodName, List<incomeT> values)
        {
            return await DoMlemRequest<List<outcomeT>>(methodName, values);
        }

        /// <summary>
        /// Predict probability method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<List<List<double>>> GetProbabilityAsync(string methodName, List<incomeT> values)
        {
            return await DoMlemRequest<List<List<double>>>(methodName, values);
        }

        private async Task<T> DoMlemRequest<T>(string methodName, List<incomeT> values)
        {
            ValidateMethod(methodName);

            ValidateValues(values);

            var argsName = _apiDescription.Methods.First(m => m.MethodName == methodName).ArgsName;

            var jsonRequest = RequestBuilder<incomeT>.BuildRequest(argsName, values);

            return await DoPostRequest<T>(methodName, jsonRequest);
        }

        private async Task<T> DoPostRequest<T>(string command, string requestJsonString)
        {

            _logger.LogInformation($"Request command: {command}");

            string responseMessage;
            try
            {
                var response = await _httpClient.PostAsync(
                    command,
                    new StringContent(requestJsonString, Encoding.UTF8, MediaTypeNames.Application.Json));

                _logger.LogInformation($"Response status: {response.StatusCode}.");

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

                        _logger.LogWarning($"Response deserialization result is null.");
                    }

                    return result;
                }
                catch
                {
                    _logger.LogError($"Response deserialization error.");

                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API request error.");

                throw;
            }
        }

        private void ValidateMethod(string methodName)
        {
            if (!_apiDescription.Methods.Any(m => m.MethodName == methodName))
            {
                var message = $"No method {methodName} in API.";

                _logger.LogError(message);

                throw new InvalidOperationException(message);
            }
        }

        private void ValidateValues(List<incomeT> values)
        {
            if (values == null)
            {
                _logger.LogError($"Input value is null: {nameof(values)}.");

                throw new ArgumentNullException(nameof(values));
            }

            if (!values.Any())
            {
                _logger.LogError($"Input value is empty: {nameof(values)}.");

                throw new ArgumentException($"{nameof(values)} cannot be empty.");
            }
        }
    }
}
