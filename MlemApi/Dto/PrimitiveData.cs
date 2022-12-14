namespace MlemApi.Dto
{
    /// <summary>
    /// Common interface for primitive types in mlem model api schema
    /// </summary>
    internal class PrimitiveData : IApiDescriptionDataStructure
    {
        public string Ptype { get; set; }
    }
}
