﻿﻿using MlemApi.Dto;
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
        private readonly string dtoDataframeTemplatePath = Path.Combine("ClassesGenerator", "DtoDataframeTemplate.template");
        private readonly string dtoNdarrayTemplatePath = Path.Combine("ClassesGenerator", "DtoNdarrayTemplate.template");

        private string dtoDataframeTemplateContent;
        private string dtoNdarrayTemplateContent;

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

        private void GenerateRequestClasses(string camelCasedModelName, string classAccessModifier, string namespaceName, MethodDescription method, StubbleVisitorRenderer templateRenderer, string outputPath, RenderSettings renderSettings)
        {
            var typeAlias = camelCaseConverter.ConvertToCamelCase($"{camelCasedModelName}{camelCaseConverter.ConvertToCamelCase(method.MethodName)}RequestType");

            if (method.ArgsData is DataFrameData)
            {
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
            Directory.CreateDirectory(outputPath);

            var apiDescription = mlemApiClient.GetDescription();
            var camelCasedModelName = camelCaseConverter.ConvertToCamelCase(modelName);

            dtoDataframeTemplateContent = File.ReadAllText(dtoDataframeTemplatePath);
            dtoNdarrayTemplateContent = File.ReadAllText(dtoNdarrayTemplatePath);

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
                     renderSettings
                 );

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
