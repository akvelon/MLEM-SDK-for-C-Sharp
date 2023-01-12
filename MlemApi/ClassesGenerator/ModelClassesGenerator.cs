using Microsoft.Extensions.Logging;
using MlemApi.Dto;
using MlemApi.Dto.DataFrameData;
using MlemApi.Utils;
using Stubble.Core;
using Stubble.Core.Builders;
using Stubble.Core.Settings;

namespace MlemApi.ClassesGenerator
{
    /// <summary>
    /// Allows to generate request/response classes for specified client as static files
    /// </summary>
    public class ModelClassesGenerator
    {
        private readonly ILogger? _logger;
        private CamelCaseConverter _camelCaseConverter = new CamelCaseConverter();
        private IPrimitiveTypeHelper _primitiveTypeHelper = new PrimitiveTypeHelper();
        private TemplatesProvider _templatesProvider = new TemplatesProvider();

        private string _dtoDataframeTemplateContent;
        private string _dtoNdarrayTemplateContent;

        public ModelClassesGenerator(ILogger? logger = null)
        {
            this._logger = logger;
        }

        /// <summary>
        /// Generates string representation of multi-dimensional list
        /// </summary>
        /// <param name="shape">Shape of list to be generated - list of sizes for each array dimension</param>
        /// <param name="elementType">Type of element in list</param>
        /// <returns>String representation of the list</returns>
        private string GetMultiDimentionalListTypeString(List<int?> shape, string elementType)
        {
            if (shape.Count == 0)
            {
                return elementType;
            }

            return $"List<{GetMultiDimentionalListTypeString(shape.GetRange(1, shape.Count - 1), elementType)}>";
        }

        /// <summary>
        /// Writes classes to files
        /// </summary>
        /// <param name="outputDirectory">Output directory for generated classes</param>
        /// <param name="classFileName">Name of the generated class</param>
        /// <param name="fileContent">File content</param>
        private void WriteClassToFile(string outputDirectory, string classFileName, string fileContent)
        {
            _logger?.LogDebug($"Writing {classFileName} to {outputDirectory}");
            File.WriteAllText(
                    Path.Combine(outputDirectory, $"{classFileName}.cs"),
                    fileContent
                );
            _logger?.LogDebug($"Successfully written {classFileName} to file");
        }

        /// <summary>
        /// Generates classes for request data
        /// </summary>
        /// <param name="camelCasedModelName">Name of the mlem model in camel case</param>
        /// <param name="classAccessModifier">Access modifier of the class</param>
        /// <param name="namespaceName">Namespace name</param>
        /// <param name="method">Method description object</param>
        /// <param name="templateRenderer">Template renderer</param>
        /// <param name="outputDirectory">Output directory for generated classes</param>
        /// <param name="renderSettings">Renderer settings</param>
        private void GenerateRequestClasses(string camelCasedModelName, string classAccessModifier, string namespaceName, MethodDescription method, StubbleVisitorRenderer templateRenderer, string outputDirectory, RenderSettings renderSettings)
        {
            _logger?.LogDebug($"Generating classes for request type");
            var typeAlias = _camelCaseConverter.ConvertToCamelCase($"{camelCasedModelName}{_camelCaseConverter.ConvertToCamelCase(method.MethodName)}RequestType");

            if (method.ArgsData.DataType is DataFrameData)
            {
                _logger?.LogDebug($"Generating for dataframe type...");
                GenerateDataframeClass(
                    classAccessModifier,
                    namespaceName,
                    method.ArgsData.DataType as DataFrameData,
                    templateRenderer,
                    outputDirectory,
                    typeAlias,
                    renderSettings
                );
            }
            else if (method.ArgsData.DataType is NdarrayData)
            {
                _logger?.LogDebug($"Generating for ndarray type...");
                GenerateNdArrayClass(
                    classAccessModifier,
                    namespaceName,
                    method.ArgsData.DataType as NdarrayData,
                    templateRenderer,
                    outputDirectory,
                    typeAlias,
                    renderSettings
                );
            }
        }

        /// <summary>
        /// Generates classes for response
        /// </summary>
        /// <param name="camelCasedModelName">Name of the mlem model in camel case</param>
        /// <param name="classAccessModifier">Access modifier of the class</param>
        /// <param name="namespaceName">Namespace name</param>
        /// <param name="method">Method description object</param>
        /// <param name="templateRenderer">Template renderer</param>
        /// <param name="outputDirectory">Output directory for generated classes</param>
        /// <param name="renderSettings">Renderer settings</param>
        private void GenerateResponseClasses(string camelCasedModelName, string classAccessModifier, string namespaceName, MethodDescription method, StubbleVisitorRenderer templateRenderer, string outputDirectory, RenderSettings renderSettings)
        {
            _logger?.LogDebug($"Generating classes for response type");
            GenerateNdArrayClass(
                classAccessModifier,
                namespaceName,
                method.ReturnData.DataType as NdarrayData,
                templateRenderer,
                outputDirectory,
                $"{camelCasedModelName}{_camelCaseConverter.ConvertToCamelCase(method.MethodName)}ResponseType",
                renderSettings
            );
        }

        /// <summary>
        /// Generates classes in static files - for request and response data
        /// 
        /// </summary>
        /// <param name="modelName">Some descriptive name for relevant mlem model. Will be coerced to camel case and used for naming</param>
        /// <param name="outputDirectory">Output directory for generated files</param>
        /// <param name="mlemApiClient">Mlem client - to get schema</param>
        /// <param name="namespaceName">Namespace name for classes to be generated</param>
        /// <param name="classAccessModifier">Access modifier for generated class</param>
        public void GenerateClasses(string modelName, string outputDirectory, MlemApiClient mlemApiClient, string namespaceName, string classAccessModifier = "internal")
        {
            _logger?.LogInformation($"Started classes generating for model {modelName}, output path - {outputDirectory}");
            _logger?.LogDebug($"Namespace name: {namespaceName}");
            _logger?.LogDebug($"Namespace name: {classAccessModifier}");

            Directory.CreateDirectory(outputDirectory);

            var apiDescription = mlemApiClient.GetDescription();
            var camelCasedModelName = _camelCaseConverter.ConvertToCamelCase(modelName);

            _dtoDataframeTemplateContent = _templatesProvider.DtoDataframeTemplate;
            _dtoNdarrayTemplateContent = _templatesProvider.DtoNdarrayTemplate;

            foreach (var method in apiDescription.Methods)
            {
                _logger?.LogDebug($"Generating classes for method {method.MethodName}");
                var templateRenderer = new StubbleBuilder().Build();
                var renderSettings = new RenderSettings
                {
                    SkipHtmlEncoding = true
                };

                _logger?.LogDebug($"Generating request type classes...");
                GenerateRequestClasses(
                     camelCasedModelName,
                     classAccessModifier,
                     namespaceName,
                     method,
                     templateRenderer,
                     outputDirectory,
                     renderSettings
                 );

                _logger?.LogDebug($"Generating response type classes...");
                GenerateResponseClasses(
                     camelCasedModelName,
                     classAccessModifier,
                     namespaceName,
                     method,
                     templateRenderer,
                     outputDirectory,
                     renderSettings
                 );
            }
        }

        /// <summary>
        /// Generates classes for Ndarray type
        /// </summary>
        /// <param name="classAccessModifier">Access modifier of the class</param>
        /// <param name="namespaceName">Namespace name</param>
        /// <param name="ndArrayData">Scheme data for ndarray</param>
        /// <param name="templateRenderer">Template renderer</param>
        /// <param name="outputDirectory">Output directory for generated files</param>
        /// <param name="typeAlias">Type alias for ndarray type</param>
        /// <param name="renderSettings">Renderer settings</param>
        private void GenerateNdArrayClass(string classAccessModifier, string namespaceName, NdarrayData ndArrayData, StubbleVisitorRenderer templateRenderer, string outputDirectory, string typeAlias, RenderSettings renderSettings)
        {
            _logger?.LogDebug($"Generating class for ndarray type");
            var stringifiedNdArrayDimensions = ndArrayData.Shape
               .Select(dimensionSize => dimensionSize == null ? "" : dimensionSize.ToString());

            var templateResponseInputData = new
            {
                AccessModifier = classAccessModifier,
                NamespaceName = namespaceName,
                TypeAlias = typeAlias,
                DataStructureType = GetMultiDimentionalListTypeString(
                    ndArrayData.Shape.ToList(),
                    _primitiveTypeHelper.GetMappedDtype(ndArrayData.Dtype)
                ),
            };

            string renderedClassContent = templateRenderer.Render(_dtoNdarrayTemplateContent, templateResponseInputData, renderSettings);

            WriteClassToFile(outputDirectory, templateResponseInputData.TypeAlias, renderedClassContent);
        }

        /// <summary>
        /// Generates classes for dataframe type
        /// </summary>
        /// <param name="classAccessModifier">Access modifier of the class</param>
        /// <param name="namespaceName">Namespace name</param>
        /// <param name="dataFrameData">Scheme data for dataframe</param>
        /// <param name="templateRenderer">Template renderer</param>
        /// <param name="outputDirectory">Output directory for generated files</param>
        /// <param name="typeAlias">Type alias for dataframe type</param>
        /// <param name="renderSettings">Renderer settings</param>
        private void GenerateDataframeClass(string classAccessModifier, string namespaceName, DataFrameData dataFrameData, StubbleVisitorRenderer templateRenderer, string outputDirectory, string typeAlias, RenderSettings renderSettings)
        {
            _logger?.LogDebug($"Generating class for dataframe type");
            var columnsData = from columnData in dataFrameData.ColumnsData
                              select new ColumnData
                              {
                                  NameInModel = columnData.Name,
                                  NameInClass = _camelCaseConverter.ConvertToCamelCase(columnData.Name),
                                  TypeInClass = _primitiveTypeHelper.GetMappedDtype(columnData.Dtype)
                              };

            var templateInputData = new
            {
                AccessModifier = classAccessModifier,
                NamespaceName = namespaceName,
                ClassName = typeAlias,
                ColumnsData = columnsData.ToList(),
            };

            var renderedClassContent = templateRenderer.Render(_dtoDataframeTemplateContent, templateInputData, renderSettings);

            WriteClassToFile(outputDirectory, templateInputData.ClassName, renderedClassContent);
        }
    }
}
