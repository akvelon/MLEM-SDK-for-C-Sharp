using MlemApi;
using MlemApi.Validation.Exceptions;
using MlemApiClientTests.Utilities;
using ModelRepository.InvalidRequestObjects;
using ModelRepository.SampleRequestObjects;

namespace MlemApiClientTests.IntegrationTests.IrisTests
{
    public class PredictTests : BaseTests
    {
        [Test]
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        [TestCase(false, true)]
        public async Task PredictAsync_ReturnsExpected_Count(bool useRequestValidation, bool useResponseValidation)
        {
            _client.ArgumentTypesValidationIsOn = useRequestValidation;
            _client.ResponseValidationIsOn = useResponseValidation;

            List<long>? result = await _client.PredictAsync<List<long>, Iris>(
                GetIrisDataList(),
                ValidationMaps.irisColumnsMap
            );

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task PredictAsync_ProvidesExpectedLogs()
        {
            CustomTestLogger logger = new();
            MlemApiClient client = GetClientWithCustomLogger(logger);

            var result = await client.PredictAsync<List<long>, Iris>(
                GetIrisDataList(),
                ValidationMaps.irisColumnsMap
            );

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(logger.Logs.Count, Is.EqualTo(4));

            List<string> expectedLogs = new()
            {
                "Request command: interface.json",
                "Request command: predict",
@"Request JSON string: {""data"": {""values"": [{
  ""sepal length (cm)"": 5.1,
  ""sepal width (cm)"": 3.5,
  ""petal length (cm)"": 1.4,
  ""petal width (cm)"": 0.2
},{
  ""sepal length (cm)"": 7.6,
  ""sepal width (cm)"": 3.0,
  ""petal length (cm)"": 6.6,
  ""petal width (cm)"": 2.1
}]}}",
                "Response status: OK.",
            };

            Assert.That(logger.Logs, Is.EquivalentTo(expectedLogs));
        }

        [Test]
        public void PredictAsync_WithSingleRequestObject_ThrowsArgumentNullException_If_Argument_IsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                () => _client.PredictAsync<List<long>, Iris?>((Iris?)null));
        }

        [Test]
        public void PredictAsync_WithRequestObjectsList_ThrowsArgumentNullException_If_Argument_IsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                () => _client.PredictAsync<List<long>, Iris>((IEnumerable<Iris>?)null));
        }

        [Test]
        public void PredictAsync_ThrowsArgumentNullException_If_ArgumentList_IsEmpty()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _client.PredictAsync<List<long>, Iris>(
                new List<Iris>(), // empty list
                ValidationMaps.irisColumnsMap
            ));
        }

        [Test]
        public void PredictAsync_ThrowsInvalidTypeException_If_RequestObject_HasUnexpectedPropertyType()
        {
            _client.ArgumentTypesValidationIsOn = true;

            Assert.ThrowsAsync<InvalidTypeException>(() => _client.PredictAsync<List<long>, IrisWithInvalidArgumentType?>(
                new List<IrisWithInvalidArgumentType>
                {
                    new IrisWithInvalidArgumentType
                    {
                        SepalLength = 1,
                        SepalWidth = 3.5,
                        PetalLength = 1.4,
                        PetalWidth = 0.2
                    },
                    new IrisWithInvalidArgumentType
                    {
                        SepalLength = 1,
                        SepalWidth = 3.0,
                        PetalLength = 6.6,
                        PetalWidth = 2.1
                    }
                },
                ValidationMaps.irisColumnsMap
            ));
        }

        [Test]
        public void PredictAsync_ThrowsKeyNotFoundException_If_RequestObject_HasMissingColumn()
        {
            _client.ArgumentTypesValidationIsOn = true;

            Assert.ThrowsAsync<KeyNotFoundException>(() => _client.PredictAsync<List<long>, IrisWithMissingColumn?>(
                new List<IrisWithMissingColumn>
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
            ));
        }

        [Test]
        public void PredictAsync_ThrowsKeyNotFoundException_If_RequestObject_HasUnknownColumn()
        {
            _client.ArgumentTypesValidationIsOn = true;

            Assert.ThrowsAsync<KeyNotFoundException>(() => _client.PredictAsync<List<long>, IrisWithUnknownColumnName>(
                new List<IrisWithUnknownColumnName>
                {
                    new IrisWithUnknownColumnName
                    {
                        SepalLength = 5.1,
                        SepalWidth = 3.5,
                        PetalLength = 1.4,
                        Unknown = 3.5
                    },
                    new IrisWithUnknownColumnName
                    {
                        SepalLength = 7.6,
                        SepalWidth = 3.0,
                        PetalLength = 6.6,
                        Unknown = 3.5
                    }
                },
                ValidationMaps.irisColumnsMap
            ));
        }

        [Test]
        public void PredictAsync_ThrowsIllegalArrayNestingLevel_ForIncorrectResponseNesting()
        {
            MlemApiClient client = GetClientWithMockedHttpClient("[[1,2]]");
            client.ResponseValidationIsOn = true;

            var exception = Assert.ThrowsAsync<IllegalArrayNestingLevelException>(() => client.PredictAsync<List<long>, Iris>(
                GetIrisDataList(),
                ValidationMaps.irisColumnsMap
            ));

            Assert.That(exception.Message, Is.EqualTo("Unexpected level of nesting in response data - appeared 1, but 0 is expected as maximum"));
        }

        [Test]
        public void PredictAsync_ThrowsInvalidTypeException_ForIncorrectResponseType()
        {
            MlemApiClient client = GetClientWithMockedHttpClient("[1,\"text\"]");
            client.ResponseValidationIsOn = true;

            var exception = Assert.ThrowsAsync<InvalidTypeException>(() => client.PredictAsync<List<long>, Iris>(
                GetIrisDataList(),
                ValidationMaps.irisColumnsMap
            ));

            Assert.That(exception.Message, Is.EqualTo("Value 'text' is not compatible with expected type Int64"));
        }

        [Test]
        [Ignore("Validation doesn't work correctly for this case. " +
            "It doesn't throw the exception in this case.")]
        public void PredictAsync_ThrowsIllegalArrayLength_ForIncorrectResponseSize()
        {
            MlemApiClient client = GetClientWithMockedHttpClient("[1,4,10,2,5]");
            client.ResponseValidationIsOn = true;

            var exception = Assert.ThrowsAsync<IllegalArrayLengthException>(() => client.PredictAsync<List<long>, Iris>(
                GetIrisDataList(),
                ValidationMaps.irisColumnsMap
            ));

            Assert.That(exception.Message.Contains("does not have expected length - actual is 4, but 3 expected"), Is.EqualTo(true));
        }
    }
}