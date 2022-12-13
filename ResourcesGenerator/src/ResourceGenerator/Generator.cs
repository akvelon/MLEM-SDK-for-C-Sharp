using Newtonsoft.Json.Linq;
using ResourceGenerator.Interfaces;
using ResourceGenerator.Models;
using Stubble.Core.Builders;
using Stubble.Core.Settings;
using static ResourceGenerator.Interfaces.IGenerator;

namespace ResourceGenerator
{
    class Generator : IGenerator
    {
        public void Generate(string jsonResourceFilesDirectory, string outputDirectoryPath, ClientTypeEnum clientType)
        {
            var templateRenderer = new StubbleBuilder().Build();

            var renderSettings = new RenderSettings
            {
                SkipHtmlEncoding = true
            };

            TemplateConstants.JsonFilesData.ForEach(fileData =>
            {
                try
                {
                    GenerateJsonResourceFile(
                        jsonResourceFilesDirectory,
                        outputDirectoryPath,
                        clientType,
                        fileData,
                        templateRenderer,
                        renderSettings
                    );
                } catch ( Exception ex )
                {
                    Console.WriteLine($"Some error while generating class for file {fileData.FileName}: {ex.Message}. Continue to generate for other resource files.");
                }
            });
        }

        private void GenerateJsonResourceFile(
            string jsonResourceFilesDirectory,
            string outputDirectoryPath,
            ClientTypeEnum clientType,
            JsonResourceFileData fileData,
            Stubble.Core.StubbleVisitorRenderer templateRenderer,
            RenderSettings renderSettings
            )
        {
            using StreamReader reader = File.OpenText(Path.Combine(jsonResourceFilesDirectory, fileData.FileName));

            string json = reader.ReadToEnd();

            // JObject might be changed to some system class to avoid using Newtonsoft library
            JObject jObject = JObject.Parse(json);

            var templateResponseInputData = new
            {
                StringConstantsData = jObject.Properties()
                    .Select(x => new StringConstantData { Key = x.Name, Value = x.Value.ToString() })
                    .ToArray(),
                CommentBlock = (clientType == ClientTypeEnum.Net ? fileData.NetCommentBlock : fileData.JavaCommentBlock),
                ClassName = fileData.ClassName,
            };

            string filePath = Path.Combine(
                outputDirectoryPath,
                $"{fileData.ClassName}.{(clientType == ClientTypeEnum.Net ? "cs" : "java")}"
            );

            var fileTemplate = clientType == ClientTypeEnum.Net
                ? TemplateConstants.NetFileTemplate
                : TemplateConstants.JavaFileTemplate;

            File.WriteAllText(filePath, templateRenderer.Render(fileTemplate, templateResponseInputData, renderSettings));
            File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.ReadOnly);

            var clientName = (clientType == ClientTypeEnum.Net ? ".NET" : "Java");

            Console.WriteLine($"{clientName} client string resources file has been successfully updated! Path - {filePath}");
        }
    }
}