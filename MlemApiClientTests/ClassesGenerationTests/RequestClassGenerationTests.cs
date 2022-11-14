namespace MlemApiClientTests.ClassesGenerationTests
{
    internal class RequestClassGenerationTests : BaseTests
    {
        [Test]
        public async Task GeneratedClasses_DataframeAsArgument()
        {
            var mockedClient = TestMockUtils.GetClientWithMockedSchema(defaultSchemaJsonPath, "", 2);

            modelClassesGenerator.GenerateClasses(
                "TestModel",
                generatedClassesDirectoryPath,
                mockedClient,
                "testNamespaceName"
            );

            AssertGeneratedClassesDirectory("GeneratedClasses_DataframeAsArgument");
        }

        [Test]
        public async Task GeneratedClasses_NdArrayAsArgument()
        {
            var mockedClient = TestMockUtils.GetClientWithMockedSchema(ndArrayArgsSchemaJsonPath, "", 2);

            modelClassesGenerator.GenerateClasses(
                "TestModel",
                generatedClassesDirectoryPath,
                mockedClient,
                "testNamespaceName"
            );

            AssertGeneratedClassesDirectory("GeneratedClasses_NdArrayAsArgument");
        }

        [Test]
        public async Task GeneratedClasses_ModelName_ShouldBeCamelCased()
        {
            var mockedClient = TestMockUtils.GetClientWithMockedSchema(defaultSchemaJsonPath, "", 2);

            modelClassesGenerator.GenerateClasses(
                "Test_model",
                generatedClassesDirectoryPath,
                mockedClient,
                "testNamespaceName"
            );

            AssertGeneratedClassesDirectory("GeneratedClasses_DataframeAsArgument");
        }
    }
}
