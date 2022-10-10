using ModelGenerator.Example;
using ModelGenerator;

namespace MlemApiClientTests.SvmModelTests
{
    public class PredictTests : BaseTests
    {
        [Test]
        public async Task PositiveTest()
        {
            var result = await _client.PredictAsync<List<double>>(
                new List<SvmModel>
                {
                    new SvmModel
                    {
                        Value = 0
                    }
                });

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.That(result.Count, Is.EqualTo(1));
        }

        [Test]
        public void NullValueTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _client.PredictAsync<List<double>>((RequestModelType?)null));
        }

        [Test]
        public void EmptyValueTest()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _client.PredictAsync<List<double>>(new List<SvmModel>()));
        }

        [Test]
        public void IncorrectMethodTest()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => _client.CallAsync<List<long>>("predict_1", new List<SvmModel>()));
        }
    }
}