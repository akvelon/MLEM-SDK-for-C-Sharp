namespace MlemApi.Utils
{
    /// <summary>
    /// Interface for classes providing operation over primitive data types supported by mlem
    /// </summary>
    internal interface IPrimitiveTypeHelper
    {
        /// <summary>
        /// Returns mapped .NET type for dType (retrieved from mlem model api schema)
        /// </summary>
        /// <param name="dType"></param>
        /// <returns></returns>
        public string GetMappedDtype(string dType);

        /// <summary>
        /// Validates value type based on expectedDtype
        /// </summary>
        /// <param name="value"></param>
        /// <param name="expectedDtype"></param>
        public void ValidateType(string value, string expectedDtype);
    }
}
