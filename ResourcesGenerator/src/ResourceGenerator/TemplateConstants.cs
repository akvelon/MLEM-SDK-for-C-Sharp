using ResourceGenerator.Models;

namespace ResourceGenerator
{
    internal static class TemplateConstants
    {
        /// <summary>
        /// Json resource files - should be the same as for common-resources repo
        /// </summary>
        public static readonly List<JsonResourceFileData> JsonFilesData = new List<JsonResourceFileData>()
        {
            new()
            {
                FileName = "Error_messages.json",
                ClassName = "EM",
                NetCommentBlock = @"/// <summary>
    /// EM (Error Messages) is a collection of string constants, used to define exception messaged.
    /// It's an auto-generated file from `ResourcesGenerator\CommonResources\Error_messages.json` file,
    /// that is the shared string resource between .NET and Java clients and allows to sync exception messaged between them.
    /// Use these constans derictly or with string.Format(...) if a resource string requests arguments.
    /// </summary>",
                JavaCommentBlock = @"/**
* EM (Error Messages) is a collection of string constants, used to define exception messaged.
* It's an auto-generated file from `ResourcesGenerator\CommonResources\Error_messages.json` file,
* that is the shared string resource between .NET and Java clients and allows to sync exception messaged between them.
* Use these constans derictly or with string.Format(...) if a resource string requests arguments.
*/",
            },
            new()
            {
                FileName = "Log_messages.json",
                ClassName = "LM",
                NetCommentBlock = @"/// <summary>
    /// LM (Log Messages) is a collection of string constants, used to define log messages.
    /// It's an auto-generated file from `ResourcesGenerator\CommonResources\Log_messages.json` file,
    /// that is the shared string resource between .NET and Java clients and allows to sync common string resources between them.
    /// Use these constans derictly or with MessageFormat.format(...) if a resource string requests arguments.
    /// </summary>",
                JavaCommentBlock = @"/**
* LM (Log Messages) is a collection of string constants, used to define log messages.
* It's an auto-generated file from `ResourcesGenerator\CommonResources\Log_messages.json` file,
* that is the shared string resource between .NET and Java clients and allows to sync common string resources between them.
* Use these constans derictly or with MessageFormat.format(...) if a resource string requests arguments.
*/",
            },
        };

        public static readonly string NetFileTemplate =
@"// Auto-generated.
// Do NOT change this file manually. To add or change any resource, change the submodule.

namespace MlemApi.MessageResources
{
    {{CommentBlock}}
    internal static class {{ClassName}}
    { 
    {{#StringConstantsData}}
        public const string {{Key}} = ""{{Value}}"";
    {{/StringConstantsData}}
    }
}
";
        public static readonly string JavaFileTemplate = @"// Auto-generated.
// Do NOT change this file manually. To add or change any resource, change the submodule.

package com.akvelon.client.resources;

{{CommentBlock}}
public final class {{ClassName}} {
    {{#StringConstantsData}}
    public static final String {{Key}} = ""{{Value}}"";
    {{/StringConstantsData}}
}
";

    }
}
