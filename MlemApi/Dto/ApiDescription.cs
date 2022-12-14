namespace MlemApi.Dto
{
    /// <summary>
    /// Represens api schema of mlem model
    /// </summary>
    internal class ApiDescription
    {
        internal ApiDescription()
        {
            Methods = new List<MethodDescription>();
        }

        /// <summary>
        /// Methods list - provided by api schema
        /// </summary>
        internal List<MethodDescription> Methods { get; set; }
    }
}
