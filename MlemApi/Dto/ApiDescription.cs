namespace MlemApi.Dto
{
    /// <summary>
    /// Represens api schema of mlem model
    /// </summary>
    public class ApiDescription
    {
        public ApiDescription()
        {
            Methods = new List<MethodDescription>();
        }

        /// <summary>
        /// Methods list - provided by api schema
        /// </summary>
        public List<MethodDescription> Methods { get; set; }
    }
}
