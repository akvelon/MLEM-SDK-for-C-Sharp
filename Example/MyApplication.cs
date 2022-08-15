using MlemApi;
using Microsoft.Extensions.Logging;

namespace Example
{
    internal class MyApplication
    {
        private readonly MlemApiClient _mlemApiClient;

        public MyApplication(IHttpClientFactory httpClientFactory, IMlemApiConfiguration configuraion, IRequestValueSerializer requestSerializer, ILogger<MlemApiClient> logger)
        {
            _mlemApiClient = new MlemApiClient(httpClientFactory.CreateClient("MlemApiClient"), configuraion, requestSerializer, logger);
        }

        public async Task<List<long>> Run()
        {
            return await _mlemApiClient.PredictAsync<Iris, List<long>>(
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
    }
}
