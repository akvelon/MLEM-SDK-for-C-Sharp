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
        private readonly string HEROKU_APP_URL = "https://example-mlem-get-started-app.herokuapp.com";
        private readonly string LOCAL_URL = "http://127.0.0.1:8080";
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
                case TestCases.LightGbm:
                    await RunLightGbmCase();
                    break;
                case TestCases.TorchTensor:
                    await RunTorchTensorCase();
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private async Task RunSingleIrisCase()
        {
            MlemApiClient mlemClient = GetMlemClient(HEROKU_APP_URL, logger: _logger, throwErrorIfUnsupportedSchemaVersion: false, requestSerializer: _requestSerializer);

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
            MlemApiClient mlemClient = GetMlemClient(HEROKU_APP_URL, logger: _logger, requestSerializer: _requestSerializer);

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
            MlemApiClient mlemClient = GetMlemClient(HEROKU_APP_URL, requestSerializer: _requestSerializer);

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
            var fileLogger = new FileLogger("./iris-log.txt");
            MlemApiClient mlemClient = GetMlemClient(HEROKU_APP_URL, logger: fileLogger, requestSerializer: _requestSerializer);

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
            HttpClient httpClient = _httpClientFactory.CreateClient(nameof(MlemApiClient));
            MlemApiClient mlemClient = GetMlemClient(HEROKU_APP_URL, requestSerializer: _requestSerializer);

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
            MlemApiClient mlemClient = GetMlemClient(HEROKU_APP_URL, requestSerializer: _requestSerializer);

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
            HttpClient httpClient = _httpClientFactory.CreateClient(nameof(MlemApiClient));
            MlemApiClient mlemClient = GetMlemClient(HEROKU_APP_URL, requestSerializer: _requestSerializer);

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
            MlemApiClient mlemClient = GetMlemClient(LOCAL_URL, logger: _logger, argumentTypesValidationIsOn: true);
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
            MlemApiClient mlemClient = GetMlemClient(LOCAL_URL);

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
            MlemApiClient client = GetMlemClient(HEROKU_APP_URL, argumentTypesValidationIsOn: true);
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
            MlemApiClient client = GetMlemClient(HEROKU_APP_URL, argumentTypesValidationIsOn: true);
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
            MlemApiClient client = GetMlemClient(HEROKU_APP_URL, argumentTypesValidationIsOn: true );
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
            MlemApiClient mlemClient = GetMlemClient(LOCAL_URL, argumentTypesValidationIsOn: true);
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

        private async Task RunTorchTensorCase()
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(nameof(MlemApiClient));
            MlemApiClient mlemClient = GetMlemClient(LOCAL_URL);
            Random rand = new();

            List<float> input = new();
            for (var i = 0; i < 100; ++i)
            {
                input.Add(rand.NextSingle());
            }

            var predict = await mlemClient.PredictAsync<List<List<float>>, List<float>>(input);

            ShowResult<float>(predict);
        }

        public void RunClassGeneration()
        {
            var modelGenerator = new ModelClassesGenerator(_logger);
            var client = GetMlemClient(LOCAL_URL);
            modelGenerator.GenerateClasses("digits", "generatedClassesFolder", client, "CustomNamespace");
        }

        public async Task RunTextModelCase()
        {
            MlemApiClient mlemClient = GetMlemClient(LOCAL_URL, requestSerializer: _requestSerializer);
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
            MlemApiClient mlemClient = GetMlemClient(LOCAL_URL, requestSerializer: _requestSerializer);
            mlemClient.ResponseValidationIsOn = false;
            var schema = mlemClient.GetDescription();

            Console.WriteLine("\nMethods:");
            foreach (var method in schema.Methods)
            {
                Console.WriteLine(" - "+ method.MethodName);
            }
        }

        private MlemApiClient GetMlemClient(
            string Url,
            ILogger<MlemApiClient>? logger = null,
            IRequestValuesSerializer? requestSerializer = null,
            bool argumentTypesValidationIsOn = false,
            bool throwErrorIfUnsupportedSchemaVersion = false
        )
        {
            HttpClient httpClient = _httpClientFactory.CreateClient(nameof(MlemApiClient));
            return new(Url, logger, httpClient, requestSerializer, null, argumentTypesValidationIsOn, throwErrorIfUnsupportedSchemaVersion);
        }

        private async Task RunLightGbmCase()
        {
            var mlemClient = GetMlemClient(LOCAL_URL, requestSerializer: _requestSerializer);

            Random rand = new();

            var input = Enumerable.Range(0, 28).Select((v, i) => new { v = rand.NextSingle() * 10, i })
                            .ToDictionary(p => p.i + 1, p => p.v);

            ShowResult<double>(await mlemClient.PredictAsync<List<double>, Dictionary<int, float>>(
                input
            ));
        }

        private static void ShowResult<T>(IEnumerable? result)
        {
            switch (result)
            {
                case null:
                    Console.WriteLine("Result is null");
                    break;
                case List<T> res:
                    {
                        Console.Write("Result: ");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(string.Join(",", res));
                        Console.ForegroundColor = ConsoleColor.White;
                        break;
                    }
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
