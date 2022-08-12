using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using MlemApi;


namespace MlemApiClientTests
{
    public class Test
    {
        /*protected MlemApiClient<Iris, long> _client;

        [SetUp]
        public void Setup()
        {
            var configurationMock = new Mock<IMlemApiConfiguration>();
            configurationMock.Setup(c => c.Url).Returns("http://127.0.0.1:8080");

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<MlemApiClient<Iris, long>>();


            Func<HttpRequestMessage, bool> isPredictMethod = m =>
            {
                return m.RequestUri.LocalPath == "predict";
            };

            var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync", 
                    ItExpr.Is<HttpRequestMessage>(m =>
                    {
                        return m.RequestUri.LocalPath == "predict";
                    }), 
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("")
                });

            var client = new HttpClient(mockHttpMessageHandler.Object);

            _client = new MlemApiClient<Iris, long>(client, configurationMock.Object, logger);
        }

        [Test]
        public async Task TestPredictSuccess()
        {
            var result = await _client.PredictAsync(
                new List<Iris>
                {
                    new Iris
                    {
                        SepalLength = -69639435.20838484,
                        SepalWidth = 64887767.01179123,
                        PetalLength = -76043679.89193763,
                        PetalWidth = 20142568.61724788
                    },
                    new Iris
                    {
                        SepalLength = 6343387.454046518,
                        SepalWidth = -30195626.60490367,
                        PetalLength = 64042930.90411937,
                        PetalWidth = -69196204.98948716
                    }
                });

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 2);
        }*/
    }
}
