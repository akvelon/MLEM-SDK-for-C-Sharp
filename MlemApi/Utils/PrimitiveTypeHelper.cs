﻿using MlemApi.MessageResources;
﻿using System.ComponentModel;
using MlemApi.Validation.Exceptions;

namespace MlemApi.Utils
{
    /// <summary>
    /// Provides operation over primitive data types supported by mlem
    /// </summary>
    internal class PrimitiveTypeHelper : IPrimitiveTypeHelper
    {
        /// <summary>
        /// List of types supported (.NET types)
        /// </summary>
        public enum SupportedTypes {
            Single,
            Double,
            SByte,
            Int16,
            Int32,
            Int64,
            Byte,
            UInt16,
            UInt32,
            UInt64,
            Boolean,
            String,
        };

        /// <summary>
        /// Map of Numpy types (used in sklearn) to C# primitive types
        /// See https://gist.github.com/robbmcleod/73ca42da5984e6d0e5b6ad28bc4a504efor for the referenced list of types
        /// Please note that .NET types mentioned here should support Parse method (except for String type) and be System types
        /// (See ValidateType method implementation)
        /// </summary>
        private readonly Dictionary<string, SupportedTypes> _typesMap = new Dictionary<string, SupportedTypes>()
        {
            { "float32", SupportedTypes.Single },
            { "float64", SupportedTypes.Double },
            { "int8", SupportedTypes.SByte },
            { "int16", SupportedTypes.Int16 },
            { "int32", SupportedTypes.Int32 },
            { "int64", SupportedTypes.Int64 },
            { "uint8", SupportedTypes.Byte },
            { "uint16", SupportedTypes.UInt16 },
            { "uint32", SupportedTypes.UInt32 },
            { "uint64", SupportedTypes.UInt64 },
            { "bool", SupportedTypes.Boolean },
            { "str", SupportedTypes.String },
        };

        /// <summary>
        /// Returns relevant .NET type type for specified dtype
        /// </summary>
        /// <param name="dType">dtype from model</param>
        /// <returns>Relevant .NET type</returns>
        public string GetMappedDtype(string dType)
        {
            try
            {
                return _typesMap[dType].ToString();
            }
            catch (KeyNotFoundException)
            {
                throw new KeyNotFoundException(string.Format(EM.UnknownValueType, dType));
            }
        }

        /// <summary>
        /// Validates if value is of expected dtype
        /// </summary>
        /// <param name="value">value to be validated</param>
        /// <param name="expectedDtype">expected dtype from model</param>
        /// <param name="parseStringValue">If true - parses value as string, otherwise - tries to assess it's type</param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="FormatException"></exception>
        public void ValidateType<T>(T value, string expectedDtype, bool parseStringValue)
        {
            var expectedNetTypeStr = GetMappedDtype(expectedDtype);

            var expectedNetType = Type.GetType($"System.{expectedNetTypeStr}");
            if (parseStringValue)
            {
                try
                {
                    if (expectedNetType != typeof(String))
                    {
                        var typeConverter = TypeDescriptor.GetConverter(expectedNetType);
                        typeConverter.ConvertFrom(value.ToString());
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidTypeException($"Value '{value}' is not compatible with expected type - {expectedNetType}", e);
                }
            }
            else
            {
                Type valueType = null;

                try
                {
                    valueType = value.GetType();
                }
                catch (Exception e)
                {
                    throw new InvalidTypeException($"Can't retrieve runtime type for value {value} and validate", e);
                }

                if (valueType != expectedNetType)
                {
                    throw new InvalidTypeException($"Incorrect type - current is {valueType.Name}, but {expectedNetTypeStr} expected");
                }
            }
        }
    }
}
