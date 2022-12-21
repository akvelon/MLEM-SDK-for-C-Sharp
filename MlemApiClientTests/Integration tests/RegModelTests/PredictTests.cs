using MlemApi.Validation.Exceptions;
using ModelRepository.SampleRequestObjects;

namespace MlemApiClientTests.RegModelTests
{
    public class PredictTests : BaseTests
    {
        [Test]
        public async Task PositiveTest()
        {
            var result = await _client.PredictAsync<List<double>, SvmModel>(
                new List<SvmModel>
                {
                    new SvmModel
                    {
                        Value = 0
                    }
                },
                ValidationMaps.svmModelMap
            );

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void NullValueTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _client.PredictAsync<List<double>, SvmModel?>((SvmModel?)null));
        }

        [Test]
        public void EmptyValueTest()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _client.PredictAsync<List<double>, SvmModel>(
                new List<SvmModel>(), ValidationMaps.irisColumnsMap
            ));
        }

        [Test]
        public void IncorrectMethodTest()
        {
            Assert.ThrowsAsync<IllegalPathException>(() => _client.CallAsync<List<long>, SvmModel>(
                "predict_1",
                new List<SvmModel>(),
                ValidationMaps.svmModelMap
            ));
        }
    }
}