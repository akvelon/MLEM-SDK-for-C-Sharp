using System.Net.Mime;
using System.Text;
using System.Text.Json;
using MlemApi.Dto;
using Microsoft.Extensions.Logging;

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

            _httpClient.BaseAddress = new Uri(_configuraion.ConnectionString);
        }

        /// <summary>
        /// Predict method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<List<outcomeT>> PredictAsync(List<incomeT> values)
        {
            return await DoCommonMlemRequest<List<outcomeT>>("predict", values);
        }

        /// <summary>
        /// Predict probability method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<List<List<double>>> PredictProbabilityAsync(List<incomeT> values)
        {
            return await DoCommonMlemRequest<List<List<double>>>("predict_proba", values);
        }

        /// <summary>
        /// Sklearn predict method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<List<outcomeT>> SklearnPredictAsync(List<incomeT> values)
        {
            return await DoSklearnMlemRequest<List<outcomeT>>("sklearn_predict", values);
        }

        /// <summary>
        /// Sklearn predict probability method
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<List<List<double>>> SklearnPredictProbabilityAsync(List<incomeT> values)
        {
            return await DoSklearnMlemRequest<List<List<double>>>("sklearn_predict_proba", values);
        }

        private void Validate(List<incomeT> values)
        {
            if (values == null)
            {
                _logger.LogError($"Input value is null: {nameof(values)}");

                throw new ArgumentNullException(nameof(values));
            }

            if (!values.Any())
            {
                _logger.LogError($"Input value is empty: {nameof(values)}");

                throw new ArgumentException($"{nameof(values)} cannot be empty");
            }
        }

        private async Task<T> DoCommonMlemRequest<T>(string command, List<incomeT> values)
        {
            Validate(values);

            var request = new CommonPredictRequest<incomeT> { Data = new Data<incomeT> { Values = values } };

            return await DoRequest<CommonPredictRequest<incomeT>, T>(command, request);
        }

        private async Task<T> DoSklearnMlemRequest<T>(string command, List<incomeT> values)
        {
            Validate(values);

            var request = new SklearnPredictRequest<incomeT> { Data = new Data<incomeT> { Values = values } };

            return await DoRequest<SklearnPredictRequest<incomeT>, T>(command, request);
        }

        private async Task<outT> DoRequest<inT, outT>(string command, inT value)
        {
            var requestJsonString = JsonSerializer.Serialize(
                value,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            _logger.LogInformation($"Request command: {command}");

            string responseMessage;
            try
            {
                var response = await _httpClient.PostAsync(
                    command,
                    new StringContent(requestJsonString, Encoding.UTF8, MediaTypeNames.Application.Json));

                _logger.LogInformation($"Response status: {response.StatusCode}");

                responseMessage = await response.Content.ReadAsStringAsync();

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpRequestException(responseMessage, null, response.StatusCode);
                }

                try
                {
                    var result = JsonSerializer.Deserialize<outT>(responseMessage);

                    if (result == null)
                    {

                        _logger.LogWarning($"Response deserialization result is null");
                    }

                    return result;
                }
                catch
                {
                    _logger.LogError($"Response deserialization error");

                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"API request error");

                throw;
            }
        }
    }
}
