using Microsoft.Extensions.Logging;
using Moq;
using MlemApi;

namespace MlemApiClientTests.TorchTensorTests
{
    public abstract class BaseTests
    {
        protected MlemApiClient _client;
        protected IMlemApiConfiguration _mlemApiConfiguration;

        [SetUp]
        public void Setup()
        {
            var configurationMock = new Mock<IMlemApiConfiguration>();
            configurationMock.Setup(c => c.Url).Returns("http://127.0.0.1:8080");
            _mlemApiConfiguration = configurationMock.Object;

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<MlemApiClient>();

            _client = new MlemApiClient(new HttpClient(), _mlemApiConfiguration, new NewtonsoftRequestValueSerializer(), logger);
        }
    }
}
