using System.Text.Json;
using MlemApi.Dto;
using MlemApi.Dto.DataFrameData;
using MlemApi.Parsing;
using static System.Text.Json.JsonElement;

namespace MlemApi.DataTypeParsers
{
    internal class PandasTypesProvider : IDataTypeProvider
    {
        public List<string> GetSupportedTypes()
        {
            return new List<string>() { "dataframe" };
        }

        public IApiDescriptionDataStructure GetTypeFromSchema(JsonElement.ObjectEnumerator objectEnumerator, IDataTypeProvider childDataTypeProvider)
        {
            var argumentNames = objectEnumerator.First(e => e.Name == "columns")
             .Value.EnumerateArray()
             .Select(element => element.GetString())
             .ToList();

            var argumentTypes = objectEnumerator.First(e => e.Name == "dtypes")
              .Value.EnumerateArray()
              .Select(element => element.GetString())
              .ToList();

            var columnsData = argumentNames.Select((argumentName, index) => new DataFrameColumnData
            {
                Name = argumentName,
                Dtype = argumentTypes[index],
            });

            return new DataFrameData
            {
                ColumnsData = columnsData,
            };
        }
    }
}
