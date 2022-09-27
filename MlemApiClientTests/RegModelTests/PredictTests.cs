namespace MlemApiClientTests.RegModelTests
{
    public class PredictTests : BaseTests
    {
        [Test]
        public async Task PositiveTest()
        {
            var result = await _client.PredictAsync<RegModel, List<double>>(
                new List<RegModel>
                {
                    new RegModel
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
            Assert.ThrowsAsync<ArgumentNullException>(() => _client.PredictAsync<RegModel, List<double>>(null));
        }

        [Test]
        public void EmptyValueTest()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _client.PredictAsync<RegModel, List<double>>(new List<RegModel>()));
        }

        [Test]
        public void IncorrectMethodTest()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => _client.CallAsync<RegModel, List<long>>("predict_1", new List<RegModel>()));
        }
    }
}