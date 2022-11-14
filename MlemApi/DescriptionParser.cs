using System.Text.Json;
using MlemApi.Dto;
using MlemApi.Dto.DataFrameData;
using MlemApi.Validation.Exceptions;

namespace MlemApi
{
    internal static class DescriptionParser
    {
        public static ApiDescription GetApiDescription(string jsonStringDescription)
        {
            using JsonDocument jsonDocument = JsonDocument.Parse(jsonStringDescription);
            JsonElement jsonMethodElements = jsonDocument.RootElement.GetProperty("methods");
            JsonElement.ObjectEnumerator jsonMethodElementsEnumerator = jsonMethodElements.EnumerateObject();
            ApiDescription description = new(jsonMethodElementsEnumerator.Count());

            foreach (JsonProperty jsonMethodElement in jsonMethodElementsEnumerator)
            {
                try
                {
                    var argsObject = jsonMethodElement.Value
                        .EnumerateObject().First(e => e.Name == "args").Value
                        .EnumerateArray().First()
                        .EnumerateObject();

                    var returnDataObject = jsonMethodElement.Value
                            .EnumerateObject().First(e => e.Name == "returns").Value.EnumerateObject();

                    description.Methods.Add(new MethodDescription
                    (
                        methodName: jsonMethodElement.Name,
                        argsName: argsObject.First(e => e.Name == "name").Value.GetString(),
                        argsData: GetArgsData(argsObject),
                        returnData: GetReturnData(returnDataObject)
                    ));
                }
                catch (Exception ex)
                {
                    throw new InvalidApiSchemaException("Invalid api schema", ex);
                }
            }

            return description;
        }

        private static NdarrayData GetNdarrayData(JsonElement.ObjectEnumerator objectEnumerator)
        {
            JsonElement.ArrayEnumerator shapeArray = objectEnumerator.First(e => e.Name == "shape")
                .Value.EnumerateArray();

            string? dType = objectEnumerator.First(e => e.Name == "dtype")
                .Value.GetString();

            List<int?> shapeList = shapeArray
                .Select<JsonElement, int?>(shapeElement =>
                {
                    if (!int.TryParse(shapeElement.ToString(), out int shapeNumericValue))
                    {
                        return null;
                    }

                    return shapeNumericValue;
                })
                .ToList();

            return new NdarrayData()
            {
                Shape = shapeList,
                Dtype = dType,
            };
        }

        private static DataFrameData GetDataFrameData(JsonElement.ObjectEnumerator objectEnumerator)
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

        private static IApiDescriptionDataStructure GetArgsData(JsonElement.ObjectEnumerator argsObjectEnumerator)
        {
            var typesDataObject = argsObjectEnumerator.First(e => e.Name == "type_")
                .Value.EnumerateObject();

            var dataType = typesDataObject.First(e => e.Name == "type")
               .Value.ToString();

            return dataType switch
            {
                "dataframe" => GetDataFrameData(typesDataObject),
                "ndarray" => GetArgumentNdArrayData(),
                _ => throw new ArgumentException($"Uknown method arguments data: {dataType}")
            };

            IApiDescriptionDataStructure GetArgumentNdArrayData()
            {
                var argumentNdArrayData = GetNdarrayData(typesDataObject);
                var shapeList = argumentNdArrayData.Shape as List<int?>;
                argumentNdArrayData.Shape = shapeList.GetRange(1, shapeList.Count - 1);
                return argumentNdArrayData;
            }
        }

        private static NdarrayData GetReturnData(JsonElement.ObjectEnumerator returnObjectEnumerator)
        {
            // Considering that the only return data possible is ndarray
            return GetNdarrayData(returnObjectEnumerator);
        }
    }
}
