using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using MlemApi;
using ModelGenerator.Example;

namespace MlemApiClientTests.Benchmark
{
    public class PredictBenchmarkTests
    {
        public class PredictAPIRequestBenchmark
        {
            private readonly MlemApiClient _mlemClient;
            private readonly List<Iris> irises12;
            private readonly List<Iris> irises4_1;
            private readonly List<Iris> irises4_2;
            private readonly List<Iris> irises4_3;
            private readonly Iris _irisItem;

            public PredictAPIRequestBenchmark()
            {
                // Prepare testing data 
                _irisItem = new()
                {
                    SepalLength = -69639435.20838484,
                    SepalWidth = 64887767.01179123,
                    PetalLength = -76043679.89193763,
                    PetalWidth = 20142568.61724788
                };

                string url = "https://example-mlem-get-started-app.herokuapp.com";
                _mlemClient = new MlemApiClient(url) { ArgumentTypesValidationIsOn = false};
                Random random = new();
                irises12 = new();
                irises4_1 = new();
                irises4_2 = new();
                irises4_3 = new();

                FillCollection(irises4_1);
                FillCollection(irises4_2);
                FillCollection(irises4_3);

                // Fill the collection of 12 irises using the previous results
                irises12.AddRange(irises4_1);
                irises12.AddRange(irises4_2);
                irises12.AddRange(irises4_3);

                double GetRandomNumberInRange(double minNumber, double maxNumber)
                    => random.NextDouble() * (maxNumber - minNumber) + minNumber;

                void FillCollection(List<Iris> collection)
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        collection.Add(new()
                        {
                            SepalLength = GetRandomNumberInRange(-60000000.0000, -70000000.0000),
                            SepalWidth = GetRandomNumberInRange(60000000.0000, 70000000.0000),
                            PetalLength = GetRandomNumberInRange(-70000000.0000, -80000000.0000),
                            PetalWidth = GetRandomNumberInRange(20000000.0000, 30000000.0000)
                        });
                    }
                }
            }

            [Benchmark]
            public async Task TestSingleRequest()
            {
                await _mlemClient.PredictAsync<List<long>, Iris>(irises12);
            }

            [Benchmark]
            public async Task TestMultithreadRequest()
            {
                // Use the variables to avoid extra performance drops when using a List of Tasks
                Task task1 = _mlemClient.PredictAsync<List<long>, Iris>(_irisItem);
                Task task2 = _mlemClient.PredictAsync<List<long>, Iris>(_irisItem);
                Task task3 = _mlemClient.PredictAsync<List<long>, Iris>(_irisItem);
                Task task4 = _mlemClient.PredictAsync<List<long>, Iris>(_irisItem);
                Task task5 = _mlemClient.PredictAsync<List<long>, Iris>(_irisItem);
                Task task6 = _mlemClient.PredictAsync<List<long>, Iris>(_irisItem);
                Task task7 = _mlemClient.PredictAsync<List<long>, Iris>(_irisItem);
                Task task8 = _mlemClient.PredictAsync<List<long>, Iris>(_irisItem);
                Task task9 = _mlemClient.PredictAsync<List<long>, Iris>(_irisItem);
                Task task10 = _mlemClient.PredictAsync<List<long>, Iris>(_irisItem);
                Task task11 = _mlemClient.PredictAsync<List<long>, Iris>(_irisItem);
                Task task12 = _mlemClient.PredictAsync<List<long>, Iris>(_irisItem);

                await Task.WhenAll(task1, task2, task3, task4, task5, task6, task7, task8, task9, task10, task11, task12);
            }

            [Benchmark]
            public async Task TestTripleRequest()
            {
                Task task1 = _mlemClient.PredictAsync<List<long>, Iris>(irises4_1);
                Task task2 = _mlemClient.PredictAsync<List<long>, Iris>(irises4_2);
                Task task3 = _mlemClient.PredictAsync<List<long>, Iris>(irises4_3);

                await Task.WhenAll(task1, task2, task3);
            }
        }

        [Test]
        public void RunPredictAPIRequestBenchmark()
        {
            Summary result = BenchmarkRunner.Run<PredictAPIRequestBenchmark>(ManualConfig
                    .Create(DefaultConfig.Instance)
                    // Disable validation to run the benchmark in unit tests
                    .WithOptions(ConfigOptions.DisableOptimizationsValidator));

            Assert.That(result.BenchmarksCases, Has.Length.EqualTo(3));

            double singleRequestMeanTime = result.Reports[0].ResultStatistics.Mean;
            double multithreadRequestMeanTime = result.Reports[1].ResultStatistics.Mean;
            double tripleRequestMeanTime = result.Reports[2].ResultStatistics.Mean;

            Assert.True(singleRequestMeanTime < tripleRequestMeanTime);
            Assert.True(tripleRequestMeanTime < multithreadRequestMeanTime);
        }
    }
}
