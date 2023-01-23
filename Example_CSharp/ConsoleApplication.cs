using MlemApi;
using Microsoft.Extensions.Logging;
using MlemApi.Serializing;
using MlemApi.ClassesGenerator;
using Example.Utilities;
using ModelRepository.SampleRequestObjects;
using ModelRepository.InvalidRequestObjects;
using System.Collections;

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
                case TestCases.MultipleIrisPredictProba:
                    await RunMultipleIrisPredictProbaCase();
                    break;
                case TestCases.MultipleIrisSkLearnPredict:
                    await RunMultipleIrisSkLearnPredictCase();
                    break;
                case TestCases.MultipleIrisSkLearnPredictProba:
                    await RunMultipleIrisSkLearnPredictProbaCase();
                    break;
                case TestCases.Wine:
                    await RunSingleWineCase();
                    break;
                case TestCases.SvmModel:
                    await RunSvmCase();
                    break;
                case TestCases.IrisRequestCheckMissingColumn:
                    await RunIrisRequestCheckMissingColumnCase();
                    break;
                case TestCases.IrisRequestCheckUnknownColumn:
                    await RunIrisRequestCheckUnknownColumnCase();
                    break;
                case TestCases.IrisRequestCheckInvalidArgument:
                    await RunIrisRequestCheckInvalidArgumentCase();
                    break;
                case TestCases.DigitsRandomForest:
                    await RunDigitsCase();
                    break;
                case TestCases.ClassGeneration:
                    RunClassGeneration();
                    break;
                case TestCases.TextModel:
                    await RunTextModelCase();
                    break;
                case TestCases.CustomConsoleLoggerCase:
                    await RunCustomConsoleLoggerCase();
                    break;
                case TestCases.ApiSchemaUsage:
                    RunApiSchemaUsage();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private async Task RunSingleIrisCase()
        {
            string url = "https://example-mlem-get-started-app.herokuapp.com";
            HttpClient httpClient = _httpClientFactory.CreateClient(nameof(MlemApiClient));
            MlemApiClient mlemClient = new(url, null, httpClient, _requestSerializer);

            Iris input = new()
            {
                SepalLength = 5.1,
                SepalWidth = 3.5,
                PetalLength = 1.4,
                PetalWidth = 0.2
            };

            ShowResult<long>(await mlemClient.PredictAsync<List<long>, Iris>(
                input,
                ValidationMaps.irisColumnsMap
            ));
        }

        private async Task RunCustomConsoleLoggerCase()
        {
            string url = "https://example-mlem-get-started-app.herokuapp.com";
            HttpClient httpClient = _httpClientFactory.CreateClient(nameof(MlemApiClient));
            MlemApiClient mlemClient = new(url, _logger, httpClient, _requestSerializer);

            Iris input = new()
            {
                SepalLength = 5.1,
                SepalWidth = 3.5,
                PetalLength = 1.4,
                PetalWidth = 0.2
            };

            ShowResult<long>(await mlemClient.PredictAsync<List<long>, Iris>(
                input,
                ValidationMaps.irisColumnsMap
            ));
        }

        private async Task RunMultipleIrisCase()
        {
            string url = "https://example-mlem-get-started-app.herokuapp.com";
            HttpClient httpClient = _httpClientFactory.CreateClient(nameof(MlemApiClient));
            MlemApiClient mlemClient = new(url, null, httpClient, _requestSerializer, null, true);

            List<Iris> inputData = new()
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
                }
            };

            ShowResult<long>(await mlemClient.PredictAsync<List<long>, Iris>(
                inputData,
                ValidationMaps.irisColumnsMap
            ));
        }

        private async Task RunIrisFileLoggerCase()
        {
            string url = "https://example-mlem-get-started-app.herokuapp.com";
            HttpClient httpClient = _httpClientFactory.CreateClient(nameof(MlemApiClient));
            var fileLogger = new FileLogger("./iris-log.txt");
            MlemApiClient mlemClient = new(url, fileLogger, httpClient, _requestSerializer);

            Iris input = new()
            {
                SepalLength = 5.1,
                SepalWidth = 3.5,
                PetalLength = 1.4,
                PetalWidth = 0.2
            };

            ShowResult<long>(await mlemClient.PredictAsync<List<long>, Iris>(
                input,
                ValidationMaps.irisColumnsMap
            ));
        }

        private async Task RunMultipleIrisPredictProbaCase()
        {
            string url = "https://example-mlem-get-started-app.herokuapp.com";
            HttpClient httpClient = _httpClientFactory.CreateClient(nameof(MlemApiClient));
            MlemApiClient mlemClient = new(url, null, httpClient, _requestSerializer, null, true);

            List<Iris> inputData = new()
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
                }
            };

            ShowResult<double>(await mlemClient.CallAsync<List<List<double>>, Iris>(
                "predict_proba",
                inputData,
                ValidationMaps.irisColumnsMap
            ));
        }

        private async Task RunMultipleIrisSkLearnPredictCase()
        {
            string url = "https://example-mlem-get-started-app.herokuapp.com";
            HttpClient httpClient = _httpClientFactory.CreateClient(nameof(MlemApiClient));
            MlemApiClient mlemClient = new(url, null, httpClient, _requestSerializer, null, true);

            List<Iris> inputData = new()
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
                }
            };

            ShowResult<long>(await mlemClient.CallAsync<List<long>, Iris>(
                "sklearn_predict",
                inputData,
                ValidationMaps.irisColumnsMap
            ));
        }

        private async Task RunMultipleIrisSkLearnPredictProbaCase()
        {
            string url = "https://example-mlem-get-started-app.herokuapp.com";
            HttpClient httpClient = _httpClientFactory.CreateClient(nameof(MlemApiClient));
            MlemApiClient mlemClient = new(url, null, httpClient, _requestSerializer, null, true);

            List<Iris> inputData = new()
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
                }
            };

            ShowResult<double>(await mlemClient.CallAsync<List<List<double>>, Iris>(
                "sklearn_predict_proba",
                inputData,
                ValidationMaps.irisColumnsMap
            ));
        }

        private async Task RunSingleWineCase()
        {
            string url = "http://127.0.0.1:8080/";
            MlemApiClient mlemClient = new(url, _logger);
            mlemClient.ArgumentsValidationIsOn = true;
            mlemClient.ResponseValidationIsOn = true;

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

            ShowResult<int>(await mlemClient.PredictAsync<List<int>, Wine>(
                input,
                ValidationMaps.wineModelMap
            ));
        }

        private async Task RunSvmCase()
        {
            string url = "http://127.0.0.1:8080/";
            MlemApiClient mlemClient = new(url, null);

            List<SvmModel> inputData = new()
            {
                new SvmModel
                {
                    Value = 0
                }
            };

            ShowResult<double>(await mlemClient.PredictAsync<List<double>, SvmModel>(
                inputData,
                ValidationMaps.svmModelMap
            ));
        }

        private async Task RunIrisRequestCheckInvalidArgumentCase()
        {
            MlemApiClient client = GetIrisClient();
            client.ArgumentsValidationIsOn = true;
            client.ResponseValidationIsOn = true;

            await client.CallAsync<List<List<double>>, IrisWithInvalidArgumentType>("predict_proba", new List<IrisWithInvalidArgumentType>
                {
                    new IrisWithInvalidArgumentType
                    {
                        SepalLength = 5, // int
                        SepalWidth = 3.5,
                        PetalLength = 1.4,
                        PetalWidth = 0.2
                    },
                    new IrisWithInvalidArgumentType
                    {
                        SepalLength = 7, // int
                        SepalWidth = 3.0,
                        PetalLength = 6.6,
                        PetalWidth = 2.1
                    }
                },
               ValidationMaps.irisColumnsMap
           );
        }

        private async Task RunIrisRequestCheckMissingColumnCase()
        {
            MlemApiClient client = GetIrisClient();
            client.ArgumentsValidationIsOn = true;
            client.ResponseValidationIsOn = true;

            await client.CallAsync<List<List<double>>, IrisWithMissingColumn>("predict_proba", new List<IrisWithMissingColumn>
                {
                    new IrisWithMissingColumn
                    {
                        SepalLength = 5.1,
                        SepalWidth = 3.5,
                        PetalLength = 1.4
                    },
                    new IrisWithMissingColumn
                    {
                        SepalLength = 7.6,
                        SepalWidth = 3.0,
                        PetalLength = 6.6
                    }
                },
               ValidationMaps.irisColumnsMap
           );
        }

        private async Task RunIrisRequestCheckUnknownColumnCase()
        {
            MlemApiClient client = GetIrisClient();
            client.ArgumentsValidationIsOn = true;
            client.ResponseValidationIsOn = true;

            await client.CallAsync<List<List<double>>, IrisWithUnknownColumnName>("predict_proba", new List<IrisWithUnknownColumnName>
                {
                    new IrisWithUnknownColumnName
                    {
                        SepalLength = 5.1,
                        SepalWidth = 3.5,
                        PetalLength = 1.4,
                        Unknown = 3.5,
                    },
                    new IrisWithUnknownColumnName
                    {
                        SepalLength = 7.6,
                        SepalWidth = 3.0,
                        PetalLength = 6.6,
                        Unknown = 3.5,
                    }
                },
               ValidationMaps.irisColumnsMap
           );
        }

        private async Task RunDigitsCase()
        {
            string url = "http://127.0.0.1:8080/";
            MlemApiClient mlemClient = new(url, null);
            mlemClient.ArgumentsValidationIsOn = true;
            mlemClient.ResponseValidationIsOn = true;

            Random rand = new Random();

            List<List<double>> inputData = new();
            List<double> internalList = new();

            for (var i = 0; i < 64; ++i)
            {
                internalList.Add(rand.NextDouble());
            }

            inputData.Add(internalList);

            ShowResult<int>(await mlemClient.PredictAsync<List<int>, List<List<double>>>(
                inputData
            ));
        }

        public void RunClassGeneration()
        {
            var modelGenerator = new ModelClassesGenerator(_logger);
            var client = new MlemApiClient("http://localhost:8080/");
            modelGenerator.GenerateClasses("digits", "generatedClassesFolder", client, "CustomNamespace");
        }

        public async Task RunTextModelCase()
        {
            string url = "http://127.0.0.1:8080/";
            HttpClient httpClient = _httpClientFactory.CreateClient("MlemApiClient");
            MlemApiClient mlemClient = new(url, null, httpClient, _requestSerializer);
            mlemClient.ResponseValidationIsOn = false;
            List<string> input = new List<string>(){
                "Hugging Face is a technology company based in New York and Paris",
            };

            var res = await mlemClient.PredictAsync<string, List<string>>(
                input
            );
            Console.WriteLine(res);
        }

        public void RunApiSchemaUsage()
        {
            string url = "http://127.0.0.1:8080/";
            HttpClient httpClient = _httpClientFactory.CreateClient("MlemApiClient");
            MlemApiClient mlemClient = new(url, null, httpClient, _requestSerializer);
            mlemClient.ResponseValidationIsOn = false;
            var schema = mlemClient.GetDescription();

            Console.WriteLine("\nMethods:");
            foreach (var method in schema.Methods)
            {
                Console.WriteLine(" - "+ method.MethodName);
            }
        }

        private MlemApiClient GetIrisClient()
        {
            string url = "https://example-mlem-get-started-app.herokuapp.com";
            HttpClient httpClient = _httpClientFactory.CreateClient(nameof(MlemApiClient));
            return new(url, null, httpClient, _requestSerializer);
        }

        private static void ShowResult<T>(IEnumerable? result)
        {
            switch (result)
            {
                case null:
                    Console.WriteLine("Result is null");
                    break;
                case List<T> res:
                    Console.Write("Result: ");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(string.Join(",", res));
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case List<List<T>> listResult:
                    {
                        Console.WriteLine("Results:");
                        for (int i = 0; i < listResult.Count; i++)
                        {
                            List<T> res = listResult[i];
                            Console.Write($"   List {i + 1}: ");
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine($"{string.Join(",", res)}");
                            Console.ForegroundColor = ConsoleColor.White;
                        }

                        break;
                    }
                case List<List<List<T>>> listOfListResult:
                    {
                        Console.WriteLine("Results:");
                        for (int i = 0; i < listOfListResult.Count; i++)
                        {
                            Console.WriteLine($"   List {i}: ");
                            List<List<T>> list = listOfListResult[i];
                            for (int j = 0; j < list.Count; j++)
                            {
                                List<T> res = list[j];
                                Console.Write($"       List {j + 1}: ");
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine($"{string.Join(",", res)}");
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        }

                        break;
                    }
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
