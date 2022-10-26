namespace MlemApi.Dto
{
    public class NdarrayData : IArrayShape, IMethodArgumentData
    {
        public IEnumerable<int?> Shape { get; set; }
        public string? Dtype { get; set; }
    }
}
