namespace MlemApi.Dto
{
    internal class ListData : IApiDescriptionDataStructure
    {
        public IEnumerable<IApiDescriptionDataStructure> Items { get; set; }
    }
}
