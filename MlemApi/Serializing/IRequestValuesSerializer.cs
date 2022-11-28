namespace MlemApi.Serializing
{
    /// <summary>
    /// Interface for class witch serialize input values to json string
    /// </summary>
    public interface IRequestValuesSerializer
    {
        /// <summary>
        /// Serialize input values to list of json string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="values">input values</param>
        /// <param name="values">request object type - ndarray or dataframe</param>
        /// <returns></returns>
        string Serialize<T>(IEnumerable<T> values, string argsName, Type argsType);
    }
}
