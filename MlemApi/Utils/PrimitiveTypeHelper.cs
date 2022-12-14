using MlemApi.MessageResources;
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
        };

        /// <summary>
        /// Map of Numpy types (used in sklearn) to C# primitive types
        /// See https://gist.github.com/robbmcleod/73ca42da5984e6d0e5b6ad28bc4a504efor for the referenced list of types
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
                throw new KeyNotFoundException($"Unknown value type - {dType}");
            }
        }

        /// <summary>
        /// Validates if value is of expected dtype
        /// </summary>
        /// <param name="value">value to be validated</param>
        /// <param name="expectedDtype">expected dtype from model</param>
        /// <exception cref="Exception"></exception>
        /// <exception cref="FormatException"></exception>
        public void ValidateType(string value, string expectedDtype)
        {
            var expectedNetType = GetMappedDtype(expectedDtype);

            try
            {
                switch (expectedNetType)
                {
                    case "Double":
                        {
                            Double.Parse(value);
                            break;
                        }
                    case "Single":
                        {
                            Single.Parse(value);
                            break;
                        }
                    case "SByte":
                        {
                            SByte.Parse(value);
                            break;
                        }
                    case "Int16":
                        {
                            Int16.Parse(value);
                            break;
                        }
                    case "Int32":
                        {
                            Int32.Parse(value);
                            break;
                        }
                    case "Int64":
                        {
                            Int64.Parse(value);
                            break;
                        }
                    case "Byte":
                        {
                            Byte.Parse(value);
                            break;
                        }
                    case "UInt16":
                        {
                            UInt16.Parse(value);
                            break;
                        }
                    case "UInt32":
                        {
                            UInt32.Parse(value);
                            break;
                        }
                    case "UInt64":
                        {
                            UInt64.Parse(value);
                            break;
                        }
                    case "Boolean":
                        {
                            Boolean.Parse(value);
                            break;
                        }
                    default:
                        {
                            throw new NotImplementedException(string.Format(EM.NoValidationLogic, expectedNetType));
                        }
                }
            }
            catch (FormatException)
            {
                throw new InvalidTypeException(string.Format(EM.InvalidType, value, expectedNetType));
            }
        }
    }
}
