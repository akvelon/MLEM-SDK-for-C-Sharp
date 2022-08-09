using Microsoft.Extensions.Logging;
using Moq;
using MlemApi;

namespace MlemApiClientTests.RegModelTests
{
    public abstract class BaseTests
    {
        protected MlemApiClient<RegModel, double> _client;
        protected IMlemApiConfiguration _mlemApiConfiguration;

        [SetUp]
        public void Setup()
        {
            var configurationMock = new Mock<IMlemApiConfiguration>();
            configurationMock.Setup(c => c.ConnectionString).Returns("http://127.0.0.1:8080");
            _mlemApiConfiguration = configurationMock.Object;

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<MlemApiClient<RegModel, double>>();

            _client = new MlemApiClient<RegModel, double>(new HttpClient(), _mlemApiConfiguration, logger);
        }
    }
}
