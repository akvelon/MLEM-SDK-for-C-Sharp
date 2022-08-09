namespace MlemApiClientTests.RegModelTests
{
    public class PredictTests : BaseTests
    {
        [Test]
        public async Task PositiveTest()
        {
            var result = await _client.PredictAsync(
                new List<RegModel>
                {
                    new RegModel
                    {
                        Zero = 0
                    }
                });

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.AreEqual(result.Count, 1);
        }

        [Test]
        public void NullValueTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _client.PredictAsync(null));
        }

        [Test]
        public void EmptyValueTest()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _client.PredictAsync(new List<RegModel>()));
        }
    }
}