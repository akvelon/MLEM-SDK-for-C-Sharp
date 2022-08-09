namespace MlemApi
{
    /// <summary>
    /// Configuration of mlem client
    /// </summary>
    public interface IMlemApiConfiguration
    {
        /// <summary>
        /// Address to connect to mlem API
        /// </summary>
        public string Url { get; set; }
    }
}
