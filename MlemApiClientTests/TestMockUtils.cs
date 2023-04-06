using System.Net;
using Microsoft.Extensions.Logging;
using MlemApi;
using Moq.Protected;
using Moq;

namespace MlemApiClientTests
{
    internal static class TestMockUtils
    {
        private static readonly string baseAddress = "https://example-mlem-get-started-app.herokuapp.com";
        internal static MlemApiClient GetClientWithMockedSchema(string pathToApiSchemaJson, string responseToSet, int countOfApiResponses = 1, int countOfModelResponses = 1)
        {
            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var sequentialMockResultSendAsync = mockHttpMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>());
            var sequentialMockResultApi = mockHttpMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>(
                 "SendAsync",
                 ItExpr.Is<HttpRequestMessage>(m => Equals(m.RequestUri, $"{TestMockUtils.baseAddress}/interface.json")),
                 ItExpr.IsAny<CancellationToken>()
             );

            for (int i = 0; i < countOfApiResponses; ++i)
            {
                var httpSchemaResponse = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(File.ReadAllText(pathToApiSchemaJson)),
                };

                sequentialMockResultApi.ReturnsAsync(httpSchemaResponse);
            }

            for (int i = 0; i < countOfModelResponses; ++i)
            {
                var httpResponse = new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseToSet),
                };

                sequentialMockResultSendAsync.ReturnsAsync(httpResponse);
            }

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<MlemApiClient>();

            return new MlemApiClient(TestMockUtils.baseAddress, logger, httpClient, null, null, false, false);
        }
    }
}
