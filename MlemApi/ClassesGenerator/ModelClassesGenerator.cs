using MlemApi.Dto.DataFrameData;
using MlemApi.Utils;
using Stubble.Core.Builders;

namespace MlemApi.ClassesGenerator
{
    public class ModelClassesGenerator
    {
        private CamelCaseConverter camelCaseConverter = new CamelCaseConverter();
        private IPrimitiveTypeHelper primitiveTypeHelper = new PrimitiveTypeHelper();

        public void GenerateClasses(string outputPath, MlemApiClient mlemApiClient, string namespaceName)
        {
            Directory.CreateDirectory(outputPath);

            var apiDescription = mlemApiClient.GetDescription();

            string template = File.ReadAllText("ClassesGenerator\\DtoTemplate.template");

            foreach (var method in apiDescription.Methods)
            {
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
                        NamespaceName = namespaceName,
                        ClassName = camelCaseConverter.ConvertToCamelCase($"{method.MethodName}RequestType"),
                        ColumnsData = columnsData.ToList(),
                    };

                    var stubble = new StubbleBuilder().Build();
                    string result = stubble.Render(template, templateInputData);

                    File.WriteAllText(
                        Path.Combine(outputPath, $"{templateInputData.ClassName}.cs"),
                        result
                    );
                }
            }
        }
    }
}
