namespace MlemApiClientTests.IrisTests
{
    public class PredictTests : BaseTests
    {
        [Test]
        public async Task PositiveTest()
        {
            var result = await _client.PredictAsync<Iris, List<long>>(
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
                });

            Assert.NotNull(result);
            Assert.IsNotEmpty(result);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public void NullValueTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _client.PredictAsync<Iris, List<long>>(null));
        }

        [Test]
        public void EmptyValueTest()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _client.PredictAsync<Iris, List<long>>(new List<Iris>()));
        }

        [Test]
        public void IncorrectMethodTest()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => _client.CallAsync<Iris, List<long>>("predict_1", new List<Iris>()));
        }
    }
}