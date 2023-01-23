namespace MlemApi.Serializing
{
    /// <summary>
    /// Interface for class witch serializes single input value to json string
    /// </summary>
    public interface IRequestValueSerializer
    {
        /// <summary>
        /// Serialize input value to json string
        /// </summary>
        /// <typeparam name="T">type of the input value </typeparam>
        /// <param name="value">input value</param>
        /// <returns>serialized json value</returns>
        string Serialize<T>(T value);
    }
}
