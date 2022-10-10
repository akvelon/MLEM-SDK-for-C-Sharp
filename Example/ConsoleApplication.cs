using MlemApi;
using Microsoft.Extensions.Logging;
using ModelGenerator.Example;

namespace Example
{
    internal class ConsoleApplication
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRequestValueSerializer _requestSerializer;
        private readonly ILogger<MlemApiClient> _logger;

        public ConsoleApplication(IHttpClientFactory httpClientFactory, IRequestValueSerializer requestSerializer, ILogger<MlemApiClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _requestSerializer = requestSerializer;
            _logger = logger;
        }

        internal async Task RunTestCaseAsync(TestCases testCase)
        {
            switch (testCase)
            {
                case TestCases.SingleIris:
                    await RunSingleIrisCase();
                    break;
                case TestCases.MultipleIris:
                    await RunMultipleIrisCase();
                    break;
                case TestCases.Wine:
                    await RunSingleWineCase();
                    break;
                case TestCases.SvmModel:
                    await RunSvmCase();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public async Task RunSingleIrisCase()
        {
            string url = "https://example-mlem-get-started-app.herokuapp.com";
            HttpClient httpClient = _httpClientFactory.CreateClient("MlemApiClient");
            MlemApiClient mlemClient = new(url, _logger, httpClient, _requestSerializer);

            Iris input = new()
            {
                SepalLength = -69639435.20838484,
                SepalWidth = 64887767.01179123,
                PetalLength = -76043679.89193763,
                PetalWidth = 20142568.61724788
            };

            ShowResult(await mlemClient.PredictAsync<List<long>>(input));
        }


        public async Task RunMultipleIrisCase()
        {
            string url = "https://example-mlem-get-started-app.herokuapp.com";
            HttpClient httpClient = _httpClientFactory.CreateClient("MlemApiClient");
            MlemApiClient mlemClient = new(url, _logger, httpClient, _requestSerializer);

            List<Iris> inputData = new()
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

            ShowResult(await mlemClient.PredictAsync<List<long>>(inputData));
        }

        public async Task RunSingleWineCase()
        {
            string url = "http://127.0.0.1:8080/";
            HttpClient httpClient = _httpClientFactory.CreateClient("MlemApiClient");
            MlemApiClient mlemClient = new(url, _logger, httpClient, _requestSerializer);

            Wine input = new()
            {
                Alcohol = 12.42,
                MalicAcid = 4.43,
                Ash = 2.73,
                AlcalinityOfAsh = 26.5,
                Magnesium = 102.0,
                TotalPhenols = 2.2,
                Flavanoids = 2.13,
                NonflavanoidPhenols = 0.43,
                Proanthocyanins = 1.71,
                ColorIntensity = 2.08,
                Hue = 0.92,
                OD280_OD315_OfDilutedWines = 3.12,
                Proline = 365.0
            };

            ShowResult(await mlemClient.PredictAsync<List<int>>(input));
        }

        public async Task RunSvmCase()
        {
            string url = "http://127.0.0.1:8080/";
            MlemApiClient mlemClient = new(url, _logger);

            List<SvmModel> inputData = new()
            {
                new SvmModel
                {
                    Value = 0
                }
            };

            ShowResult(await mlemClient.PredictAsync<List<double>>(inputData));
        }

        private void ShowResult<T>(List<T>? result)
        {
            if (result is null)
            {
                Console.WriteLine("Result is null");
            }
            else
            {
                Console.WriteLine("Result: " + string.Join(",", result));
            }
        }
    }
}
