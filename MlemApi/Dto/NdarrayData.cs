namespace MlemApi.Dto
{
    /// <summary>
    /// Interface for ndarray type in mlem model api schema
    /// </summary>
    public class NdarrayData : IArrayShape, IApiDescriptionDataStructure
    {
        public IEnumerable<int?> Shape { get; set; }

        public string? Dtype { get; set; }
    }
}
