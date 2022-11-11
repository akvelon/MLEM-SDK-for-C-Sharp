namespace MlemApi.Dto
{
    internal class ApiDescription
    {
        public ApiDescription(int jsonMethodElementsCount)
        {
            Methods = new List<MethodDescription>(jsonMethodElementsCount);
        }

        public List<MethodDescription> Methods { get; set; }
    }
}
