using MlemApi;
using Microsoft.Extensions.Logging;
using ModelGenerator.Example;

namespace Example
{
    internal class MyApplication
    {
        private readonly MlemApiClient _mlemApiClientIris;
        private readonly MlemApiClient _mlemApiClientSvm;

        public MyApplication(IHttpClientFactory httpClientFactory, IRequestValueSerializer requestSerializer, ILogger<MlemApiClient> logger)
        {
            _mlemApiClientIris = new MlemApiClient("https://example-mlem-get-started-app.herokuapp.com", logger, httpClientFactory.CreateClient("MlemApiClient"), requestSerializer);

            _mlemApiClientSvm = new MlemApiClient("http://127.0.0.1:8080/", logger);
        }

        public async Task<List<long>?> RunMultipleIrisCase()
        {
            List<Iris> testData = new()
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
            };

            return await _mlemApiClientIris.PredictAsync<List<long>>(testData);
        }

        public async Task<List<long>?> RunSingleIrisCase()
        {
            Iris item = new()
            {
                SepalLength = -69639435.20838484,
                SepalWidth = 64887767.01179123,
                PetalLength = -76043679.89193763,
                PetalWidth = 20142568.61724788
            };

            return await _mlemApiClientIris.PredictAsync<List<long>>(item);
        }

        public async Task<List<double>?> RunSvm()
        {
            return await _mlemApiClientSvm.PredictAsync<List<double>>(
                new List<SvmModel>
                {
                    new SvmModel
                    {
                        Value = 0
                    }
                });
        }
    }
}
