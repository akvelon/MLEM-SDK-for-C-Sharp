using System.Text.Json;
using MlemApi.Dto;

namespace MlemApi.Parsing.DataTypeParsers
{
    /// <summary>
    /// Gets Numpy types from schema
    /// See relevant mlem code https://github.com/iterative/mlem/blob/afb18dba1cbc3e69590caa2f2a93f99dcdddf1f1/mlem/contrib/numpy.py
    /// </summary>
    internal class NumpyTypesProvider : IDataTypeProvider
    {
        public List<string> GetSupportedTypes()
        {
            return new List<string>() { "ndarray", "torch" };
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
