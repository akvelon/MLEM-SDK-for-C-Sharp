using MlemApi;
using MlemApi.Validation.Exceptions;
using MlemApiClientTests.Utilities;
using ModelRepository.InvalidRequestObjects;
using ModelRepository.SampleRequestObjects;
using Newtonsoft.Json.Linq;

namespace MlemApiClientTests.IntegrationTests.IrisTests
{
    public class CallTests : BaseTests
    {
        [Test]
        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, false)]
        [TestCase(false, true)]
        public async Task CallAsync_ReturnsExpected_Count(bool useRequestValidation, bool useResponseValidation)
        {
            _client.ArgumentTypesValidationIsOn = useRequestValidation;
            _client.ResponseValidationIsOn = useResponseValidation;

            List<List<double>>? result = await _client.CallAsync<List<List<double>>, Iris>("predict_proba",
                GetIrisDataList(),
                ValidationMaps.irisColumnsMap
            );

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Count, Is.EqualTo(3));
            Assert.That(result[1].Count, Is.EqualTo(3));
        }

        [Test]
        public async Task CallAsync_ProvidesExcpectedLogs()
        {
            CustomTestLogger logger = new();
            MlemApiClient client = GetClientWithCustomLogger(logger);

            var result = await client.CallAsync<List<List<double>>, Iris>(
                "predict_proba",
                GetIrisDataList(),
                ValidationMaps.irisColumnsMap
            );

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[0].Count, Is.EqualTo(3));
            Assert.That(result[1].Count, Is.EqualTo(3));

            Assert.That(logger.Logs.Count, Is.EqualTo(4));

            List<string> expectedLogs = new()
            {
                "Request command: interface.json",
                "Request command: predict_proba",
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
        public void CallAsync_WithSingleRequestObject_ThrowsArgumentNullException_If_Argument_IsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                () => _client.CallAsync<List<List<double>>, Iris?>("predict_proba", (Iris?)null));
        }

        [Test]
        public void CallAsync_WithRequestObjectsList_ThrowsArgumentNullException_If_Argument_IsNull()
        {
            Assert.ThrowsAsync<ArgumentNullException>(
                () => _client.CallAsync<List<List<double>>, Iris>("predict_proba", (IEnumerable<Iris>?)null));
        }

        [Test]
        public void CallAsync_ThrowsArgumentNullException_If_ArgumentList_IsEmpty()
        {
            _client.ArgumentTypesValidationIsOn = true;
            Assert.ThrowsAsync<ArgumentException>(() => _client.CallAsync<List<List<double>>, Iris>(
                "predict_proba",
                new List<Iris>(), // empty list
                ValidationMaps.irisColumnsMap
            ));
        }

        [Test]
        public void CallAsync_ThrowsInvalidTypeException_If_RequestObject_HasUnexpectedPropertyType()
        {
            _client.ArgumentTypesValidationIsOn = true;

            Assert.ThrowsAsync<InvalidTypeException>(() => _client.CallAsync<List<List<double>>, IrisWithInvalidArgumentType?>(
                "predict_proba",
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
        public void CallAsync_ThrowsKeyNotFoundException_If_RequestObject_HasMissingColumn()
        {
            _client.ArgumentTypesValidationIsOn = true;

            Assert.ThrowsAsync<KeyNotFoundException>(() => _client.CallAsync<List<List<double>>, IrisWithMissingColumn?>(
                "predict_proba",
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
        public void CallAsync_ThrowsKeyNotFoundException_If_RequestObject_HasUnknownColumn()
        {
            _client.ArgumentTypesValidationIsOn = true;

            Assert.ThrowsAsync<KeyNotFoundException>(() => _client.CallAsync<List<List<double>>, IrisWithUnknownColumnName>(
                "predict_proba",
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
        public void CallAsync_ThrowsIllegalArrayNestingLevel_ForIncorrectResponseNesting()
        {
            MlemApiClient client = GetClientWithMockedHttpClient("[1,2]");
            client.ResponseValidationIsOn = true;

            var exception = Assert.ThrowsAsync<IllegalArrayNestingLevel>(() => client.CallAsync<List<List<double>>, Iris>(
                "predict_proba",
                GetIrisDataList(),
                ValidationMaps.irisColumnsMap
            ));

            Assert.That(exception.Message, Is.EqualTo("Primitive values on nesting level 1 appeared, but expected on 2 level only"));
        }

        [Test]
        public void CallAsync_ThrowsInvalidTypeException_ForIncorrectResponseType()
        {
            MlemApiClient client = GetClientWithMockedHttpClient("[1,\"text\"]");
            client.ResponseValidationIsOn = true;

            var exception = Assert.ThrowsAsync<InvalidTypeException>(() => client.CallAsync<List<List<double>>, Iris>(
                "predict",
                GetIrisDataList(),
                ValidationMaps.irisColumnsMap
            ));

            Assert.That(exception.Message, Is.EqualTo("Value 'text' is not compatible with expected type - Int64"));
        }

        [Test]
        public void CallAsync_ThrowsIllegalArrayLength_ForIncorrectResponseSize()
        {
            MlemApiClient client = GetClientWithMockedHttpClient("[[1,4,10,2]]");
            client.ResponseValidationIsOn = true;

            var exception = Assert.ThrowsAsync<IllegalArrayLength>(() => client.CallAsync<List<List<double>>, Iris>(
                "predict_proba",
                GetIrisDataList(),
                ValidationMaps.irisColumnsMap
            ));

            Assert.That(exception.Message.Contains("does not have expected length - actual is 4, but 3 expected"), Is.EqualTo(true));
        }

        [Test]
        public void IncorrectMethodTest()
        {
            Assert.ThrowsAsync<IllegalPathException>(() => _client.CallAsync<List<long>, Iris>(
                "predict_1",
                new List<Iris>(),
                ValidationMaps.irisColumnsMap
            ));
        }
    }
}
