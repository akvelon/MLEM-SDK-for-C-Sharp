using Microsoft.Extensions.Logging;
using Moq;
using MlemApi;

namespace MlemApiClientTests.SimplifiedClientTests
{
    public abstract class BaseTests
    {
        protected MlemApiClient _client;

        [SetUp]
        public void Setup()
        {
            _client = new MlemApiClient("https://example-mlem-get-started-app.herokuapp.com");
        }
    }
}
