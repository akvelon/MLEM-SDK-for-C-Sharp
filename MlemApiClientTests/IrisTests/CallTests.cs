using ModelGenerator.Example;
using ModelGenerator;
using MlemApiClientTests.IrisTests.TestClasses;

namespace MlemApiClientTests.IrisTests
{
    public class CallTests : BaseTests
    {
        [Test]
        public async Task PositiveTest()
        {
            var result = await _client.CallAsync<List<List<double>>>("predict_proba",
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
                },
                ModelGenerator.Sample_models.ValidationMaps.irisColumnsMap
            );

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Count, Is.EqualTo(3));
            Assert.That(result[1].Count, Is.EqualTo(3));
        }

        [Test]
        public void NullValueTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _client.CallAsync<List<List<double>>>("predict_proba", (RequestModelType?)null));
        }

        [Test]
        public void EmptyValueTest()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _client.CallAsync<List<List<double>>>("predict_proba", new List<Iris>()));
        }

        [Test]
        public void IncorrectMethodTest()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => _client.CallAsync<List<List<double>>>("predict_proba_2", new List<Iris>()));
        }

        [Test]
        public async Task ValidationCorrectTest()
        {
            _client.ArgumentTypesValidationIsOn = true;

            var result = await _client.CallAsync<List<List<double>>>("predict_proba", new List<Iris>
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
                },
               ModelGenerator.Sample_models.ValidationMaps.irisColumnsMap
           );

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Count, Is.EqualTo(3));
            Assert.That(result[1].Count, Is.EqualTo(3));
        }

        [Test]
        public void ValidationWrongArgumentTypeTest()
        {
            _client.ArgumentTypesValidationIsOn = true;

            Assert.ThrowsAsync<ArgumentException>(() => _client.CallAsync<List<List<double>>>("predict_proba", new List<IrisWithInvalidArgumentType>
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
           ));
        }

        [Test]
        public void ValidationMissingColumnTest()
        {
            _client.ArgumentTypesValidationIsOn = true;

            Assert.ThrowsAsync<ArgumentException>(() => _client.CallAsync<List<List<double>>>("predict_proba", new List<IrisWIthMissingColumn>
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
           ));
        }

        [Test]
        public void ValidationMissingNamesMapTest()
        {
            _client.ArgumentTypesValidationIsOn = true;

            Assert.ThrowsAsync<ArgumentException>(() => _client.CallAsync<List<List<double>>>("predict_proba", new List<IrisWIthMissingColumn>
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
           ));
        }

        [Test]
        public void ValidationUnknownColumnTest()
        {
            _client.ArgumentTypesValidationIsOn = true;

            Assert.ThrowsAsync<ArgumentException>(() => _client.CallAsync<List<List<double>>>("predict_proba", new List<IrisWithUnknownColumnName>
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
           ));
        }

        [Test]
        public void ResponseValidationWrongNestingLevel()
        {
            var client = GetClientWithMockedHttpClient("[1,2]");
            client.ResponseValidationIsOn = true;

            var exception = Assert.ThrowsAsync<ArgumentException>(() => client.CallAsync<List<List<double>>>("predict_proba", new List<Iris>
                {
                    new Iris
                    {
                        SepalLength = 6343387.454046518,
                        SepalWidth = 64887767.01179123,
                        PetalLength = -76043679.89193763,
                        PetalWidth = 20142568.61724788,
                    },
                    new Iris
                    {
                        SepalLength = 6343387.454046518,
                        SepalWidth = -30195626.60490367,
                        PetalLength = 64042930.90411937,
                        PetalWidth = 20142568.61724788,
                    }
                },
               ModelGenerator.Sample_models.ValidationMaps.irisColumnsMap
           ));

            Assert.That(exception.Message, Is.EqualTo("Primitive values on nesting level 1 appeared, but expected on 2 level only"));
        }

        [Test]
        public async Task ResponseValidationPositive()
        {
            var client = GetClientWithMockedHttpClient("[[1,2,3]]");
            client.ResponseValidationIsOn = true;

            var result = await client.CallAsync<List<List<double>>>("predict_proba", new List<Iris>
                {
                    new Iris
                    {
                        SepalLength = 6343387.454046518,
                        SepalWidth = 64887767.01179123,
                        PetalLength = -76043679.89193763,
                        PetalWidth = 20142568.61724788,
                    },
                    new Iris
                    {
                        SepalLength = 6343387.454046518,
                        SepalWidth = -30195626.60490367,
                        PetalLength = 64042930.90411937,
                        PetalWidth = 20142568.61724788,
                    }
                },
               ModelGenerator.Sample_models.ValidationMaps.irisColumnsMap
           );

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Count, Is.EqualTo(3));
        }

        [Test]
        public void ResponseValidationWrongValueType()
        {
            var client = GetClientWithMockedHttpClient("[1,\"text\"]");
            client.ResponseValidationIsOn = true;

            var exception = Assert.ThrowsAsync<FormatException>(() => client.CallAsync<List<List<double>>>("predict", new List<Iris>
                {
                    new Iris
                    {
                        SepalLength = 6343387.454046518,
                        SepalWidth = 64887767.01179123,
                        PetalLength = -76043679.89193763,
                        PetalWidth = 20142568.61724788,
                    },
                    new Iris
                    {
                        SepalLength = 6343387.454046518,
                        SepalWidth = -30195626.60490367,
                        PetalLength = 64042930.90411937,
                        PetalWidth = 20142568.61724788,
                    }
                },
               ModelGenerator.Sample_models.ValidationMaps.irisColumnsMap
           ));

            Assert.That(exception.Message, Is.EqualTo("Value 'text' is not compatible with expected type - Int64"));
        }

        [Test]
        public void ResponseValidationWrongArraySize()
        {
            var client = GetClientWithMockedHttpClient("[[1,4,10,2]]");
            client.ResponseValidationIsOn = true;

            var exception = Assert.ThrowsAsync<ArgumentException>(() => client.CallAsync<List<List<double>>>("predict_proba", new List<Iris>
                {
                    new Iris
                    {
                        SepalLength = 6343387.454046518,
                        SepalWidth = 64887767.01179123,
                        PetalLength = -76043679.89193763,
                        PetalWidth = 20142568.61724788,
                    },
                    new Iris
                    {
                        SepalLength = 6343387.454046518,
                        SepalWidth = -30195626.60490367,
                        PetalLength = 64042930.90411937,
                        PetalWidth = 20142568.61724788,
                    }
                },
               ModelGenerator.Sample_models.ValidationMaps.irisColumnsMap
           ));

            Assert.That(exception.Message.Contains("does not have expected length - actual is 4, but 3 expected"), Is.EqualTo(true));
        }
    }
}