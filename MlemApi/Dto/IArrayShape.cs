namespace MlemApi.Dto
{
    /// <summary>
    /// Interface for array shape data - providing info about restrictions for multi-dimensional array lengths
    /// </summary>
    public interface IArrayShape
    {
        IEnumerable<int?> Shape { get; set; }
    }
}
