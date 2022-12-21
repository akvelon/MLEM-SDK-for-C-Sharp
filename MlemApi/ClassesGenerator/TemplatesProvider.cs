namespace MlemApi.ClassesGenerator
{
    /// <summary>
    /// Provides templates for classes - request/response data types
    /// </summary>
    internal class TemplatesProvider
    {
        public string DtoDataframeTemplate = @"using Newtonsoft.Json;

namespace {{NamespaceName}}
{
    {{AccessModifier}} class {{ClassName}}
    {
        {{#ColumnsData}}
        [JsonProperty(""{{NameInModel}}"")]
        public {{TypeInClass}} {{NameInClass}} { get; set; }
        {{/ColumnsData}}
    }
}
";
        public string DtoNdarrayTemplate = @"namespace {{NamespaceName}}
{
    {{AccessModifier}} class {{TypeAlias}} : {{DataStructureType}} {}
}
";
    }
}
