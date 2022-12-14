using Microsoft.Extensions.Logging;
using MlemApi.Dto;
using MlemApi.Dto.DataFrameData;
using MlemApi.Utils;
using Stubble.Core;
using Stubble.Core.Builders;
using Stubble.Core.Settings;

namespace MlemApi.ClassesGenerator
{
    public class ModelClassesGenerator
    {
        private readonly ILogger? logger;
        private CamelCaseConverter camelCaseConverter = new CamelCaseConverter();
        private IPrimitiveTypeHelper primitiveTypeHelper = new PrimitiveTypeHelper();
        private readonly string dtoDataframeTemplatePath = Path.Combine("ClassesGenerator", "DtoDataframeTemplate.template");
        private readonly string dtoNdarrayTemplatePath = Path.Combine("ClassesGenerator", "DtoNdarrayTemplate.template");

        private string dtoDataframeTemplateContent;
        private string dtoNdarrayTemplateContent;

        public ModelClassesGenerator(ILogger? logger = null)
        {
            this.logger = logger;
        }

        private string GetMultiDimentionalListTypeString(List<int?> shape, string elementType)
        {
            if (shape.Count == 0)
            {
                return elementType;
            }

            return $"List<{GetMultiDimentionalListTypeString(shape.GetRange(1, shape.Count - 1), elementType)}>";
        }

        private void WriteClassToFile(string outputPath, string classFileName, string fileContent)
        {
            logger?.LogDebug($"Writing {classFileName} to {outputPath}");
            File.WriteAllText(
                    Path.Combine(outputPath, $"{classFileName}.cs"),
                    fileContent
                );
            logger?.LogDebug($"Successfully written {classFileName} to file");
        }

        private void GenerateRequestClasses(string camelCasedModelName, string classAccessModifier, string namespaceName, MethodDescription method, StubbleVisitorRenderer templateRenderer, string outputPath, RenderSettings renderSettings)
        {
            logger?.LogDebug($"Generating classes for request type");
            var typeAlias = camelCaseConverter.ConvertToCamelCase($"{camelCasedModelName}{camelCaseConverter.ConvertToCamelCase(method.MethodName)}RequestType");

            if (method.ArgsData is DataFrameData)
            {
                logger?.LogDebug($"Generating for dataframe type...");
                GenerateDataframeClass(
                    classAccessModifier,
                    namespaceName,
                    method.ArgsData as DataFrameData,
                    templateRenderer,
                    outputPath,
                    typeAlias,
                    renderSettings
                );
            }
            else if (method.ArgsData is NdarrayData)
            {
                logger?.LogDebug($"Generating for ndarray type...");
                GenerateNdArrayClass(
                    classAccessModifier,
                    namespaceName,
                    method.ArgsData as NdarrayData,
                    templateRenderer,
                    outputPath,
                    typeAlias,
                    renderSettings
                );
            }
        }

        private void GenerateResponseClasses(string camelCasedModelName, string classAccessModifier, string namespaceName, MethodDescription method, StubbleVisitorRenderer templateRenderer, string outputPath, RenderSettings renderSettings)
        {
            logger?.LogDebug($"Generating classes for response type");
            GenerateNdArrayClass(
                classAccessModifier,
                namespaceName,
                method.ReturnData as NdarrayData,
                templateRenderer,
                outputPath,
                $"{camelCasedModelName}{camelCaseConverter.ConvertToCamelCase(method.MethodName)}ResponseType",
                renderSettings
            );
        }

        public void GenerateClasses(string modelName, string outputPath, MlemApiClient mlemApiClient, string namespaceName, string classAccessModifier = "internal")
        {
            logger?.LogInformation($"Started classes generating for model {modelName}, output path - {outputPath}");
            logger?.LogDebug($"Namespace name: {namespaceName}");
            logger?.LogDebug($"Namespace name: {classAccessModifier}");

            Directory.CreateDirectory(outputPath);

            var apiDescription = mlemApiClient.GetDescription();
            var camelCasedModelName = camelCaseConverter.ConvertToCamelCase(modelName);

            logger?.LogInformation($"Reading templates...");
            dtoDataframeTemplateContent = File.ReadAllText(dtoDataframeTemplatePath);
            dtoNdarrayTemplateContent = File.ReadAllText(dtoNdarrayTemplatePath);

            foreach (var method in apiDescription.Methods)
            {
                logger?.LogDebug($"Generating classes for method {method.MethodName}");
                var templateRenderer = new StubbleBuilder().Build();
                var renderSettings = new RenderSettings
                {
                    SkipHtmlEncoding = true
                };

                logger?.LogDebug($"Generating request type classes...");
                GenerateRequestClasses(
                     camelCasedModelName,
                     classAccessModifier,
                     namespaceName,
                     method,
                     templateRenderer,
                     outputPath,
                     renderSettings
                 );

                logger?.LogDebug($"Generating response type classes...");
                GenerateResponseClasses(
                     camelCasedModelName,
                     classAccessModifier,
                     namespaceName,
                     method,
                     templateRenderer,
                     outputPath,
                     renderSettings
                 );
            }
        }

        private void GenerateNdArrayClass(string classAccessModifier, string namespaceName, NdarrayData ndArrayData, StubbleVisitorRenderer templateRenderer, string outputPath, string typeAlias, RenderSettings renderSettings)
        {
            logger?.LogDebug($"Generating class for ndarray type");
            var stringifiedNdArrayDimensions = ndArrayData.Shape
               .Select(dimensionSize => dimensionSize == null ? "" : dimensionSize.ToString());

            var templateResponseInputData = new
            {
                AccessModifier = classAccessModifier,
                NamespaceName = namespaceName,
                TypeAlias = typeAlias,
                DataStructureType = GetMultiDimentionalListTypeString(
                    ndArrayData.Shape.ToList(),
                    primitiveTypeHelper.GetMappedDtype(ndArrayData.Dtype)
                ),
            };

            string renderedClassContent = templateRenderer.Render(dtoNdarrayTemplateContent, templateResponseInputData, renderSettings);

            WriteClassToFile(outputPath, templateResponseInputData.TypeAlias, renderedClassContent);
        }

        private void GenerateDataframeClass(string classAccessModifier, string namespaceName, DataFrameData dataFrameData, StubbleVisitorRenderer templateRenderer, string outputPath, string typeAlias, RenderSettings renderSettings)
        {
            logger?.LogDebug($"Generating class for dataframe type");
            var columnsData = from columnData in dataFrameData.ColumnsData
                              select new ColumnData
                              {
                                  NameInModel = columnData.Name,
                                  NameInClass = camelCaseConverter.ConvertToCamelCase(columnData.Name),
                                  TypeInClass = primitiveTypeHelper.GetMappedDtype(columnData.Dtype)
                              };

            var templateInputData = new
            {
                AccessModifier = classAccessModifier,
                NamespaceName = namespaceName,
                ClassName = typeAlias,
                ColumnsData = columnsData.ToList(),
            };

            var renderedClassContent = templateRenderer.Render(dtoDataframeTemplateContent, templateInputData, renderSettings);

            WriteClassToFile(outputPath, templateInputData.ClassName, renderedClassContent);
        }
    }
}
