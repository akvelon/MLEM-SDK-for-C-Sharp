namespace MlemApi.Dto
{
    /// <summary>
    /// Interface for list data type in mlem model api schema
    /// </summary>
    internal class ListData : IApiDescriptionDataStructure
    {
        public IEnumerable<IApiDescriptionDataStructure> Items { get; set; }
    }
}
