using System.Text.Json;
using MlemApi.Dto;
using MlemApi.Dto.DataFrameArgumentData;
using MlemApi.Validation.Exceptions;

namespace MlemApi
{
    internal static class DescriptionParser
    {
        public static ApiDescription GetApiDescription(string jsonStringDescription)
        {
            using var jsonDocument = JsonDocument.Parse(jsonStringDescription);
            var jsonMethodElements = jsonDocument.RootElement.GetProperty("methods");

            var jsonMethodElementsEnumerator = jsonMethodElements.EnumerateObject();
            var description = new ApiDescription
            {
                Methods = new List<MethodDescription>(jsonMethodElementsEnumerator.Count())
            };

            foreach (var jsonMethodElement in jsonMethodElementsEnumerator)
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
                    {
                        MethodName = jsonMethodElement.Name,
                        ArgsName = argsObject.First(e => e.Name == "name").Value.GetString(),
                        ArgsData = DescriptionParser.GetArgsData(argsObject),
                        ReturnData = DescriptionParser.GetReturnData(returnDataObject),
                    });
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
            var shapeArray = objectEnumerator.First(e => e.Name == "shape")
                .Value.EnumerateArray();

            var dType = objectEnumerator.First(e => e.Name == "dtype")
                .Value.GetString();

            var shapeList = shapeArray.Select<JsonElement, int?>(shapeElement =>
            {
                if (!Int32.TryParse(shapeElement.ToString(), out int shapeNumericValue))
                {
                    return null;
                }

                return shapeNumericValue;
            }).ToList();

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

        private static IMethodArgumentData GetArgsData(JsonElement.ObjectEnumerator argsObjectEnumerator)
        {
            var typesDataObject = argsObjectEnumerator.First(e => e.Name == "type_")
                .Value.EnumerateObject();

            var dataType = typesDataObject.First(e => e.Name == "type")
               .Value.ToString();

            if (dataType == "dataframe")
            {
                return DescriptionParser.GetDataFrameData(typesDataObject);
            }
            else if (dataType == "ndarray")
            {
                var argumentNdArrayData = DescriptionParser.GetNdarrayData(typesDataObject);
                var shapeList = argumentNdArrayData.Shape as List<int?>;
                argumentNdArrayData.Shape = shapeList.GetRange(1, shapeList.Count - 1);

                return argumentNdArrayData;
            }
            else
            {
                throw new ArgumentException($"Uknown method arguments data: {dataType}");
            }
        }

        private static NdarrayData GetReturnData(JsonElement.ObjectEnumerator returnObjectEnumerator)
        {
            // Considering that the only return data possible is ndarray
            return DescriptionParser.GetNdarrayData(returnObjectEnumerator);
        }
    }
}
