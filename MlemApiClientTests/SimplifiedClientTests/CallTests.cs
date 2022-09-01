namespace MlemApiClientTests.SimplifiedClientTests
{
    public class CallTests : BaseTests
    {
        [Test]
        public async Task PositiveTest()
        {
            var result = await _client.CallAsync<Iris, List<List<double>>>("predict_proba",
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
            Assert.That(result[0].Count, Is.EqualTo(3));
            Assert.That(result[1].Count, Is.EqualTo(3));
        }

        [Test]
        public void NullValueTest()
        {
            Assert.ThrowsAsync<ArgumentNullException>(() => _client.CallAsync<Iris, List<List<double>>>("predict_proba", null));
        }

        [Test]
        public void EmptyValueTest()
        {
            Assert.ThrowsAsync<ArgumentException>(() => _client.CallAsync<Iris, List<List<double>>>("predict_proba", new List<Iris>()));
        }

        [Test]
        public void IncorrectMethodTest()
        {
            Assert.ThrowsAsync<InvalidOperationException>(() => _client.CallAsync<Iris, List<List<double>>>("predict_proba_2", new List<Iris>()));
        }
    }
}