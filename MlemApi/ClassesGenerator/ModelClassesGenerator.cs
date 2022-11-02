using System.Dynamic;
using System.Linq;
using MlemApi.Dto.DataFrameArgumentData;
using MlemApi.Utils;
using Stubble.Core.Builders;
using Stubble.Core.Settings;

namespace MlemApi.ClassesGenerator
{
    public class ModelClassesGenerator
    {
        private CamelCaseConverter camelCaseConverter = new CamelCaseConverter();
        private IPrimitiveTypeHelper primitiveTypeHelper = new PrimitiveTypeHelper();

        private string GetMultiDimentionalListTypeString(List<int?> shape, string elementType)
        {
            if (shape.Count == 0)
            {
                return elementType;
            }

            return $"List<{GetMultiDimentionalListTypeString(shape.GetRange(1, shape.Count - 1), elementType)}>";
        }

        public void GenerateClasses(string outputPath, MlemApiClient mlemApiClient, string namespaceName, string classAccessModifier = "internal")
        {
            Directory.CreateDirectory(outputPath);

            var apiDescription = mlemApiClient.GetDescription();

            string template = File.ReadAllText("ClassesGenerator\\DtoTemplate.template");
            string responseTemplate = File.ReadAllText("ClassesGenerator\\ResponseTemplate.template");

            foreach (var method in apiDescription.Methods)
            {
                var stubble = new StubbleBuilder().Build();
                var renderSettings = new RenderSettings
                {
                    SkipHtmlEncoding = true
                };

                if (method.ArgsData is DataFrameData)
                {
                    var dataFrameData = method.ArgsData as DataFrameData;

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
                        ClassName = camelCaseConverter.ConvertToCamelCase($"{method.MethodName}RequestType"),
                        ColumnsData = columnsData.ToList(),
                    };

                    string result = stubble.Render(template, templateInputData, renderSettings);

                    File.WriteAllText(
                        Path.Combine(outputPath, $"{templateInputData.ClassName}.cs"),
                        result
                    );
                }
                var stringifiedNdArrayDimensions = method.ReturnData.Shape
                    .Select(dimensionSize => dimensionSize == null ? "" : dimensionSize.ToString());

                var templateResponseInputData = new
                {
                    AccessModifier = classAccessModifier,
                    NamespaceName = namespaceName,
                    ResponseTypeAlias = $"{camelCaseConverter.ConvertToCamelCase(method.MethodName)}ResponseType",
                    DataStructureType = GetMultiDimentionalListTypeString(
                        method.ReturnData.Shape.ToList(),
                        primitiveTypeHelper.GetMappedDtype(method.ReturnData.Dtype)
                    ),
                };

                string generatedResponseClass = stubble.Render(responseTemplate, templateResponseInputData, renderSettings);

                File.WriteAllText(
                    Path.Combine(outputPath, $"{templateResponseInputData.ResponseTypeAlias}.cs"),
                    generatedResponseClass
                );
            }
        }
    }
}
