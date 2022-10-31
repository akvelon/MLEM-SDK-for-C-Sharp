using MlemApi;
using Microsoft.Extensions.Logging;
using ModelGenerator.Example;
using MlemApi.Serializing;
using MlemApi.ClassesGenerator;

namespace Example
{
    internal class ConsoleApplication
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IRequestValuesSerializer _requestSerializer;
        private readonly ILogger<MlemApiClient> _logger;

        public ConsoleApplication(IHttpClientFactory httpClientFactory, IRequestValuesSerializer requestSerializer, ILogger<MlemApiClient> logger)
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
                case TestCases.IrisFileLogger:
                    await RunIrisFileLoggerCase();
                    break;
                case TestCases.Wine:
                    await RunSingleWineCase();
                    break;
                case TestCases.SvmModel:
                    await RunSvmCase();
                    break;
                case TestCases.IrisRequestCheckMissingColumn:
                    await IrisRequestCheckMissingColumn();
                    break;
                case TestCases.IrisRequestCheckUnknownColumn:
                    await IrisRequestCheckUnknownColumn();
                    break;
                case TestCases.IrisRequestCheckInvalidArgument:
                    await IrisRequestCheckInvalidArgument();
                    break;
                case TestCases.DigitsRandomForest:
                    await RunDigitsRandomForest();
                    break;
                case TestCases.ClassGeneration:
                    RunClassGeneration();
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

            ShowResult(await mlemClient.PredictAsync<List<long>, Iris>(
                input,
                ModelGenerator.Sample_models.ValidationMaps.irisColumnsMap
            ));
        }

        public async Task RunMultipleIrisCase()
        {
            string url = "https://example-mlem-get-started-app.herokuapp.com";
            HttpClient httpClient = _httpClientFactory.CreateClient("MlemApiClient");
            MlemApiClient mlemClient = new(url, _logger, httpClient, _requestSerializer, null, true);

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

            ShowResult(await mlemClient.PredictAsync<List<long>, Iris>(
                inputData,
                ModelGenerator.Sample_models.ValidationMaps.irisColumnsMap
            ));
        }

        public async Task RunIrisFileLoggerCase()
        {
            string url = "https://example-mlem-get-started-app.herokuapp.com";
            HttpClient httpClient = _httpClientFactory.CreateClient("MlemApiClient");
            var fileLogger = new FileLogger("./iris-log.txt");
            MlemApiClient mlemClient = new(url, fileLogger, httpClient, _requestSerializer);

            Iris input = new()
            {
                SepalLength = -69639435.20838484,
                SepalWidth = 64887767.01179123,
                PetalLength = -76043679.89193763,
                PetalWidth = 20142568.61724788
            };

            ShowResult(await mlemClient.PredictAsync<List<long>, Iris>(
                input,
                ModelGenerator.Sample_models.ValidationMaps.irisColumnsMap
            ));
        }

        public async Task RunSingleWineCase()
        {
            string url = "http://127.0.0.1:8080/";
            MlemApiClient mlemClient = new(url, _logger);

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

            ShowResult(await mlemClient.PredictAsync<List<int>, Wine>(
                input,
                ModelGenerator.Sample_models.ValidationMaps.wineModelMap
            ));
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

            ShowResult(await mlemClient.PredictAsync<List<double>, SvmModel>(
                inputData,
                ModelGenerator.Sample_models.ValidationMaps.svmModelMap
            ));
        }

        public async Task IrisRequestCheckInvalidArgument()
        {
            var client = GetIrisClient();

            await client.CallAsync<List<List<double>>, IrisWithInvalidArgumentType>("predict_proba", new List<IrisWithInvalidArgumentType>
                {
                    new IrisWithInvalidArgumentType
                    {
                        SepalLength = 1,
                        SepalWidth = 64887767.01179123,
                        PetalLength = -76043679.89193763,
                        PetalWidth = 20142568.61724788
                    },
                    new IrisWithInvalidArgumentType
                    {
                        SepalLength = 1,
                        SepalWidth = -30195626.60490367,
                        PetalLength = 64042930.90411937,
                        PetalWidth = -69196204.98948716
                    }
                },
               ModelGenerator.Sample_models.ValidationMaps.irisColumnsMap
           );
        }

        public async Task IrisRequestCheckMissingColumn()
        {
            var client = GetIrisClient();
            client.ArgumentTypesValidationIsOn = true;

            await client.CallAsync<List<List<double>>, IrisWIthMissingColumn>("predict_proba", new List<IrisWIthMissingColumn>
                {
                    new IrisWIthMissingColumn
                    {
                        SepalLength = 6343387.454046518,
                        SepalWidth = 64887767.01179123,
                        PetalLength = -76043679.89193763,
                    },
                    new IrisWIthMissingColumn
                    {
                        SepalLength = 6343387.454046518,
                        SepalWidth = -30195626.60490367,
                        PetalLength = 64042930.90411937,
                    }
                },
               ModelGenerator.Sample_models.ValidationMaps.irisColumnsMap
           );
        }

        public async Task IrisRequestCheckUnknownColumn()
        {
            var client = GetIrisClient();
            await client.CallAsync<List<List<double>>, IrisWithUnknownColumnName>("predict_proba", new List<IrisWithUnknownColumnName>
                {
                    new IrisWithUnknownColumnName
                    {
                        SepalLength = 6343387.454046518,
                        SepalWidth = 64887767.01179123,
                        PetalLength = -76043679.89193763,
                        Unknown = 3.5,
                    },
                    new IrisWithUnknownColumnName
                    {
                        SepalLength = 6343387.454046518,
                        SepalWidth = -30195626.60490367,
                        PetalLength = 64042930.90411937,
                        Unknown = 3.5,
                    }
                },
               ModelGenerator.Sample_models.ValidationMaps.irisColumnsMap
           );
        }
    
        public async Task RunDigitsRandomForest()
        {
            string url = "http://127.0.0.1:8080/";
            MlemApiClient mlemClient = new(url, _logger);
            Random rand = new Random();
            List<double> inputData = new();
            for (var i = 0; i < 64; ++i)
            {
                inputData.Add(rand.NextDouble());
            }

            ShowResult(await mlemClient.PredictAsync<List<int>, List<double>>(
                inputData
            ));
        }

        public void RunClassGeneration()
        {
            var modelGenerator = new ModelClassesGenerator();
            var client = new MlemApiClient("https://example-mlem-get-started-app.herokuapp.com");
            modelGenerator.GenerateClasses("generatedClassesFolder", client, "CustomNamespace");
        }

        private MlemApiClient GetIrisClient()
        {
            string url = "https://example-mlem-get-started-app.herokuapp.com";
            HttpClient httpClient = _httpClientFactory.CreateClient("MlemApiClient");
            return new(url, _logger, httpClient, _requestSerializer);
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
