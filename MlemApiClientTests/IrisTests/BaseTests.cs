using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using MlemApi;

namespace MlemApiClientTests.IrisTests
{
    public abstract class BaseTests
    {
        protected MlemApiClient<Iris, long> _client;

        [SetUp]
        public void Setup()
        {
            var configurationMock = new Mock<IMlemApiConfiguration>();
            configurationMock.Setup(c => c.Url).Returns("https://example-mlem-get-started-app.herokuapp.com");

            using var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<MlemApiClient<Iris, long>>();

            _client = new MlemApiClient<Iris, long>(new HttpClient(), configurationMock.Object, logger);
        }
    }
}
