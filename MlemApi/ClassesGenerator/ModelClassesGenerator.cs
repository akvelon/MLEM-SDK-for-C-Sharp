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
        private CamelCaseConverter camelCaseConverter = new CamelCaseConverter();
        private IPrimitiveTypeHelper primitiveTypeHelper = new PrimitiveTypeHelper();
        private readonly string requestTypeTemplatePath = "ClassesGenerator\\DtoTemplate.template";
        private readonly string responseTypeTemplatePath = "ClassesGenerator\\ResponseTemplate.template";

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
            File.WriteAllText(
                    Path.Combine(outputPath, $"{classFileName}.cs"),
                    fileContent
                );
        }

        private void GenerateRequestClasses(string camelCasedModelName, string classAccessModifier, string namespaceName, MethodDescription method, StubbleVisitorRenderer templateRenderer, string outputPath, string requestTypeTemplate, RenderSettings renderSettings)
        {
            if (method.ArgsData is DataFrameData dataFrameData)
            {
                var columnsData = from columnData in dataFrameData.ColumnsData
                                  select new ColumnData
                                  {
                                      NameInModel = columnData.Name,
                                      NameInClass = camelCaseConverter.ConvertToCamelCase(columnData.Name),
                                      TypeInClass = primitiveTypeHelper.GetMappedDtype(columnData.Dtype)
                                  };

                var templateInputData = new DtoTemplateInputData
                {
                    AccessModifier = classAccessModifier,
                    NamespaceName = namespaceName,
                    ClassName = camelCaseConverter.ConvertToCamelCase($"{camelCasedModelName}{method.MethodName}RequestType"),
                    ColumnsData = columnsData.ToList(),
                };

                var renderedClassContent = templateRenderer.Render(requestTypeTemplate, templateInputData, renderSettings);

                WriteClassToFile(outputPath, templateInputData.ClassName, renderedClassContent);
            }
        }

        private void GenerateResponseClasses(string camelCasedModelName, string classAccessModifier, string namespaceName, MethodDescription method, StubbleVisitorRenderer templateRenderer, string outputPath, string responseTemplate, RenderSettings renderSettings)
        {
            var stringifiedNdArrayDimensions = method.ReturnData.Shape
                .Select(dimensionSize => dimensionSize == null ? "" : dimensionSize.ToString());

            var templateResponseInputData = new ResponseTemplateInputData
            {
                AccessModifier = classAccessModifier,
                NamespaceName = namespaceName,
                ResponseTypeAlias = $"{camelCasedModelName}{camelCaseConverter.ConvertToCamelCase(method.MethodName)}ResponseType",
                DataStructureType = GetMultiDimentionalListTypeString(
                    method.ReturnData.Shape.ToList(),
                    primitiveTypeHelper.GetMappedDtype(method.ReturnData.Dtype)
                ),
            };

            string renderedClassContent = templateRenderer.Render(responseTemplate, templateResponseInputData, renderSettings);

            WriteClassToFile(outputPath, templateResponseInputData.ResponseTypeAlias, renderedClassContent);
        }

        public void GenerateClasses(string modelName, string outputPath, MlemApiClient mlemApiClient, string namespaceName, string classAccessModifier = "internal")
        {
            Directory.CreateDirectory(outputPath);

            var apiDescription = mlemApiClient.GetDescription();
            var camelCasedModelName = camelCaseConverter.ConvertToCamelCase(modelName);

            string requestTypeTemplate = File.ReadAllText(requestTypeTemplatePath);
            string responseTemplate = File.ReadAllText(responseTypeTemplatePath);

            foreach (var method in apiDescription.Methods)
            {
                var templateRenderer = new StubbleBuilder().Build();
                var renderSettings = new RenderSettings
                {
                    SkipHtmlEncoding = true
                };

                GenerateRequestClasses(
                     camelCasedModelName,
                     classAccessModifier,
                     namespaceName,
                     method,
                     templateRenderer,
                     outputPath,
                     requestTypeTemplate,
                     renderSettings
                 );

                GenerateResponseClasses(
                     camelCasedModelName,
                     classAccessModifier,
                     namespaceName,
                     method,
                     templateRenderer,
                     outputPath,
                     responseTemplate,
                     renderSettings
                 );
            }
        }
    }
}
