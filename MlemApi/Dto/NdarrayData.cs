namespace MlemApi.Dto
{
    public class NdarrayData : IArrayShape, IApiDescriptionDataStructure
    {
        public IEnumerable<int?> Shape { get; set; }

        public string? Dtype { get; set; }
    }
}
