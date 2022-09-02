using MlemApi;
using Microsoft.Extensions.Logging;

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

        public async Task<List<long>> RunIris()
        {
            return await _mlemApiClientIris.PredictAsync<Iris, List<long>>(
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
        }

        public async Task<List<double>> RunSvm()
        {
            return await _mlemApiClientSvm.PredictAsync<SvmModel, List<double>>(
                new List<SvmModel>
                {
                    new SvmModel
                    {
                        Zero = 0
                    }
                });
        }
    }
}
