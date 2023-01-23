using MlemApi.ClassesGenerator;

namespace MlemApiClientTests.ClassesGenerationTests
{
    internal class BaseTests
    {
        protected ModelClassesGenerator modelClassesGenerator;
        protected readonly string generatedClassesDirectoryPath = "testGeneratedClasses";
        protected readonly string expectedClassesDirectoryPath = Path.Combine("ClassesGenerationTests", "ClassSamplesForAssertion");
        protected readonly string defaultSchemaJsonPath = Path.Combine("ClassesGenerationTests", "TestSchemasCollection", "defaultSchema.json");
        protected readonly string ndArrayArgsSchemaJsonPath = Path.Combine("ClassesGenerationTests", "TestSchemasCollection", "ndArrayArgsSchema.json");

        [SetUp]
        public void Setup()
        {
            modelClassesGenerator = new ModelClassesGenerator();
        }

        [TearDown]
        public void TearDown()
        {
            Directory.Delete(generatedClassesDirectoryPath, true);
        }

        protected void AssertGeneratedClassesDirectory(string testCaseName)
        {
            List<string> expectedFilePaths = Directory.GetFiles(Path.Combine(expectedClassesDirectoryPath, testCaseName)).ToList();
            List<string> actualFilePaths = Directory.GetFiles(Path.Combine(generatedClassesDirectoryPath)).ToList();

            Assert.That(
                actualFilePaths.Count,
                Is.EqualTo(expectedFilePaths.Count),
                "Generated files count should be equal to expected amount of files"
            );

            foreach (var actualFilePath in actualFilePaths)
            {
                var actualFileName = Path.GetFileName(actualFilePath);

                var relevantExpectedFilePathResults = expectedFilePaths.Where(path => Path.GetFileNameWithoutExtension(path) == actualFileName);
                Assert.That(relevantExpectedFilePathResults.Count, Is.EqualTo(1), "There should be 1 relevant expected file for actual generated");

                var relevantExpectedFilePath = relevantExpectedFilePathResults.First();

                var actualFileContent = File.ReadAllText(actualFilePath);
                var expectedFileContent = File.ReadAllText(relevantExpectedFilePath);

                Assert.That(actualFileContent, Is.EqualTo(expectedFileContent), "Actual generated file content should be equal to expected");
            }
        }
    }
}
