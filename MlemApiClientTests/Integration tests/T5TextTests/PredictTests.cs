using MlemApi;
using MlemApi.Validation.Exceptions;

namespace MlemApiClientTests.Integration_tests.T5TextTests
{
    internal class PredictTests : BaseTests
    {
        [Test]
        public async Task PredictAsync_SuccessValidation_ForListAndStrTypes()
        {
            MlemApiClient client = GetClientWithMockedHttpClient("Text result");
            client.ArgumentsValidationIsOn = true;
            client.ResponseValidationIsOn = true;

            List<string> input = new List<string>(){
                "Hugging Face is a technology company based in New York and Paris",
            };

            var result = await client.PredictAsync<string, List<string>>(input);

            Assert.NotNull(result);
            Assert.That(result, Is.EqualTo("Text result"));
        }

        [Test]
        public async Task PredictAsync_ThrowValidationError_ForInvalidArgument()
        {
            MlemApiClient client = GetClientWithMockedHttpClient("Text result");
            client.ArgumentsValidationIsOn = true;

            var input = new List<int>(){
                1
            };

            var exception = Assert.ThrowsAsync<InvalidTypeException>(() => client.PredictAsync<string, List<int>>(input));

            Assert.That(exception.Message, Is.EqualTo("Incorrect type - current is Int32, but String expected"));
        }

        [Test]
        public async Task PredictAsync_ThrowValidationError_ForInvalidResponse()
        {
            MlemApiClient client = GetClientWithMockedHttpClientAndCustomSchema("Text result", "t5_text_int_result.json");
            client.ResponseValidationIsOn = true;

            List<string> input = new List<string>(){
                "Hugging Face is a technology company based in New York and Paris",
            };

            var exception = Assert.ThrowsAsync<InvalidTypeException>(() => client.PredictAsync<int, List<string>>(input));

            Assert.That(exception.Message, Is.EqualTo("Value 'Text result' is not compatible with expected type - System.Int64"));
        }

        [Test]
        public async Task PredictAsync_Success_ListWithIntValues()
        {
            MlemApiClient client = GetClientWithMockedHttpClientAndCustomSchema("3", "list_int_elems_and_result.json");
            client.ResponseValidationIsOn = true;

            List<int> input = new List<int>() { 1, 2, 3, 4, 5, 6, 7 };

            var result = await client.PredictAsync<int, List<int>>(input);

            Assert.NotNull(result);
            Assert.That(result, Is.EqualTo(3));
        }
    }
}
