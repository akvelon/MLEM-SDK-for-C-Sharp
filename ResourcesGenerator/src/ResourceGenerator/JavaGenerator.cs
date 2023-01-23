using Newtonsoft.Json.Linq;
using ResourceGenerator.Interfaces;
using Stubble.Core.Builders;
using Stubble.Core.Settings;

namespace ResourceGenerator
{
    class JavaGenerator : IGenerator
    {
        private readonly string fileTemplate =
            @"// Auto-generated.
// Do NOT change this file manually. To add or change any resource, change the submodule.

package com.akvelon.client.resources;

/**
* EM (Error Messages) is a collection of string constants, used to define exception messaged.
* It's an auto-generated file common resources string constants file,
* that is the shared string resource between .NET and Java clients and allows to sync exception messaged between them.
* Use these constans derictly or with string.Format(...) if a resource string requests arguments.
*/
public final class EM { 
{{#StringConstantsData}}
    public static final String {{Key}} = ""{{Value}}"";
{{/StringConstantsData}}
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

            // Convert up to 5 arguments in a resource string.
            // If you need more arguments in the resource, change the cycle inside.
            string json = ConvertToJavaStringFormat(reader.ReadToEnd());
            JObject jObject = JObject.Parse(json);

            var templateResponseInputData = new
            {
                StringConstantsData = jObject.Properties()
                    .Select(x => new StringConstantData { Key = x.Name, Value = x.Value.ToString() })
                    .ToArray(),
            };

            string filePath = Path.Combine(outputDirectoryPath, "EM.java");
            File.WriteAllText(filePath, templateRenderer.Render(fileTemplate, templateResponseInputData, renderSettings));

            Console.WriteLine($"Java client string resources file has been successfully updated! Path - {filePath}");
        }

        private static string ConvertToJavaStringFormat(string json)
        {
            // It might be implmeneted better. Feel free to rework it if you have a good idea
            for (int i = 0; i <= 5; i++)
            {
                json = json.Replace($"{{{i}}}", $"%{i + 1}$s");
            }

            return json;
        }
    }
}