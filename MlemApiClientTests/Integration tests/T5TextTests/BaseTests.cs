using MlemApi;

namespace MlemApiClientTests.Integration_tests.T5TextTests
{
    public abstract class BaseTests
    {
        public MlemApiClient GetClientWithMockedHttpClient(string responseToSet)
        {
            return GetClientWithMockedHttpClientAndCustomSchema(responseToSet, "t5_text_test_schema.json");
        }

        public MlemApiClient GetClientWithMockedHttpClientAndCustomSchema(string responseToSet, string schemaFileName)
        {
            return TestMockUtils.GetClientWithMockedSchema(
                Path.Combine("Assets", schemaFileName),
                responseToSet
            );
        }
    }
}
