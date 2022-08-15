using Microsoft.Extensions.Logging;
using MlemApi;

namespace Example
{
    internal class LoggingDelegatingHandler : DelegatingHandler
    {
        private readonly ILogger _logger;

        public LoggingDelegatingHandler(ILogger<MlemApiClient> logger)
        { 
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Content != null)
            {
                var requestContent = await request.Content.ReadAsStringAsync();
                _logger.LogInformation($"Request: {requestContent}");
            }

            var response = await base.SendAsync(request, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation($"Response: {responseContent}");

            return response;
        }
    }
}
