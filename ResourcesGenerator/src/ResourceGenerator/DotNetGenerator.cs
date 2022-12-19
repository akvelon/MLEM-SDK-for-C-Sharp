using Newtonsoft.Json.Linq;
using ResourceGenerator.Interfaces;
using Stubble.Core.Builders;
using Stubble.Core.Settings;

namespace ResourceGenerator
{
    class DotNetGenerator : IGenerator
    {
        private readonly string fileTemplate =
@"// Auto-generated.
// Do NOT change this file manually. To add or change any resource, change the submodule.

namespace MlemApi.MessageResources
{
    /// <summary>
    /// EM (Error Messages) is a collection of string constants, used to define exception messaged.
    /// It's an auto-generated file common resources string constants file,
    /// that is the shared string resource between .NET and Java clients and allows to sync exception messaged between them.
    /// Use these constans derictly or with string.Format(...) if a resource string requests arguments.
    /// </summary>
    internal static class EM
    { 
    {{#StringConstantsData}}
        public const string {{Key}} = ""{{Value}}"";
    {{/StringConstantsData}}
    }
}
";
        public void Generate(string jsonResourceFilePath, string outputDirectoryPath)
        {
            var templateRenderer = new StubbleBuilder().Build();
            var renderSettings = new RenderSettings
            {
                SkipHtmlEncoding = true
            };

            using StreamReader reader = File.OpenText(jsonResourceFilePath);
            string json = reader.ReadToEnd();

            // JObject might be changes to some system class to avoid using Newtonsoft library
            JObject jObject = JObject.Parse(json);


            var templateResponseInputData = new
            {
                StringConstantsData = jObject.Properties()
                    .Select(x => new StringConstantData { Key = x.Name, Value = x.Value.ToString() })
                    .ToArray(),
            };

            string filePath = Path.Combine(outputDirectoryPath, "EM.cs");
            File.WriteAllText(filePath, templateRenderer.Render(fileTemplate, templateResponseInputData, renderSettings));
            File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.ReadOnly);

            Console.WriteLine($".NET client string resources file has been successfully updated! Path - {filePath}");
        }
    }
}