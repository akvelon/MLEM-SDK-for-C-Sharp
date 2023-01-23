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
        public void ValidateType<T>(T value, string expectedDtype, bool parseStringValue);
    }
}
