using System.Net;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.Logging;
using MlemApi;
using Moq;
using Moq.Protected;
using NUnit.Framework.Internal;

namespace MlemApiClientTests.IrisTests
{
    public abstract class BaseTests
    {
        protected MlemApiClient _client;

        [SetUp]
        public void Setup()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<MlemApiClient>();

            _client = new MlemApiClient("https://example-mlem-get-started-app.herokuapp.com", logger, new HttpClient(), new NewtonsoftRequestValueSerializer());
        }

        [TearDown]
        public void TearDown()
        {
            _client.ArgumentTypesValidationIsOn = false;
        }

        public MlemApiClient GetClientWithMockedHttpClient(string responseToSet)
        {
            var httpSchemaResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(File.ReadAllText("Assets/Iris_test_schema.json")),
            };

            var httpResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseToSet),
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpSchemaResponse)
                .ReturnsAsync(httpResponse);

            var httpClient = new HttpClient(mockHttpMessageHandler.Object);

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<MlemApiClient>();

            return new MlemApiClient("https://example-mlem-get-started-app.herokuapp.com", logger, httpClient, new NewtonsoftRequestValueSerializer());
        }
    }
}
