using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MlemApi.Dto;

namespace MlemApi.Parsing.DataTypeParsers
{
    internal class NumpyTypesProvider : IDataTypeProvider
    {
        public List<string> GetSupportedTypes()
        {
            return new List<string>() { "ndarray" };
        }

        public IApiDescriptionDataStructure GetTypeFromSchema(JsonElement.ObjectEnumerator objectEnumerator, IDataTypeProvider childDataTypeProvider)
        {
            var shapeArray = objectEnumerator.First(e => e.Name == "shape")
            .Value.EnumerateArray();

            var dType = objectEnumerator.First(e => e.Name == "dtype")
                .Value.GetString();

            var shapeList = shapeArray.Select<JsonElement, int?>(shapeElement =>
            {
                if (!int.TryParse(shapeElement.ToString(), out int shapeNumericValue))
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
    }
}
