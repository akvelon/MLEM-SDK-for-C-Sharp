using System.Net;
using Microsoft.Extensions.Logging;
using MlemApi;
using ModelRepository.SampleRequestObjects;
using Moq;
using Moq.Protected;
using NUnit.Framework;

namespace MlemApiClientTests.IntegrationTests.IrisTests
{
    public abstract class BaseTests
    {
        protected MlemApiClient _client;

        [SetUp]
        public void Setup()
        {
            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<MlemApiClient>();

            _client = new MlemApiClient("https://example-mlem-get-started-app.herokuapp.com", logger, new HttpClient());
        }

        [TearDown]
        public void TearDown()
        {
            _client.ArgumentTypesValidationIsOn = false;
        }

        public MlemApiClient GetClientWithMockedHttpClient(string responseToSet)
        {
            return TestMockUtils.GetClientWithMockedSchema(
                Path.Combine("Assets", "Iris_test_schema.json"),
                responseToSet
            );
        }

        public MlemApiClient GetClientWithCustomLogger(ILogger<MlemApiClient> logger)
        {
            return new MlemApiClient("https://example-mlem-get-started-app.herokuapp.com", logger, new HttpClient());
        }

        private protected List<Iris> GetIrisDataList()
            => new List<Iris>
            {
                new Iris
                {
                    SepalLength = 5.1,
                    SepalWidth = 3.5,
                    PetalLength = 1.4,
                    PetalWidth = 0.2
                },
                new Iris
                {
                    SepalLength = 7.6,
                    SepalWidth = 3.0,
                    PetalLength = 6.6,
                    PetalWidth = 2.1
                },
            };
    }
}
