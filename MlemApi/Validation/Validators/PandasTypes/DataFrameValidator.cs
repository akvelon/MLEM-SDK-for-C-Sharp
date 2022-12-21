using MlemApi.Dto;
using MlemApi.Dto.DataFrameData;
using MlemApi.Utils;
using MlemApi.Validation.Exceptions;

namespace MlemApi.Validation.Validators.PandasTypes
{
    internal class DataFrameValidator : ITypeValidator
    {
        private readonly IPrimitiveTypeHelper primitiveTypeHelper = new PrimitiveTypeHelper();

        public List<Type> SupportedApiDescriptionDataStructure()
        {
            return new List<Type>() { typeof(DataFrameData) };
        }

        public void Validate<T, D>(T value, IApiDescriptionDataStructure apiDescriptionDataStructure, ValidationParameters validationParameters = null, D additionalData = null, ITypeValidator? childTypeVallidator = null)
            where D : class?
        {
            DataFrameData dataFrameData = apiDescriptionDataStructure as DataFrameData;
            Dictionary<string, string> modelColumnToPropNamesMap = additionalData as Dictionary<string, string>;

            if (modelColumnToPropNamesMap == null)
            {
                throw new ArgumentNullException($"Map of model column names to request object properties is empty.");
            }

            var columnsCountInSchema = dataFrameData.ColumnsData.Count();
            var actualColumnsCount = value.GetType().GetProperties().Count();

            if (actualColumnsCount > columnsCountInSchema)
            {
                throw new IllegalColumnsNumberException($"Count of request object properties is not equal to properties in schema: expected {columnsCountInSchema}, but actual is {actualColumnsCount}");
            }

            var columnsData = dataFrameData.ColumnsData;

            foreach (var columnData in columnsData)
            {
                string? objPropertyName;
                dynamic propertyValue;

                try
                {
                    objPropertyName = modelColumnToPropNamesMap[columnData.Name];
                }
                catch (KeyNotFoundException)
                {
                    throw new KeyNotFoundException($"Can't find '{columnData.Name}' key in passed column names map");
                }

                try
                {
                    var property = value.GetType().GetProperty(objPropertyName);

                    if (property == null)
                    {
                        throw new ArgumentException($"Can't find '{objPropertyName}' property in request object, although it exists in schema");
                    }

                    propertyValue = property.GetValue(value);
                }
                catch (Exception e)
                {
                    throw new KeyNotFoundException($"Can't find '{objPropertyName}' property in request object, although it exists in schema");
                }

                primitiveTypeHelper.ValidateType(propertyValue, columnData.Dtype, validationParameters.ShouldValueBeParsed);
            }
        }
    }
}
