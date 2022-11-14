namespace MlemApi.Dto
{
    internal class ApiDescription
    {
        internal ApiDescription(int jsonMethodElementsCount)
        {
            Methods = new List<MethodDescription>(jsonMethodElementsCount);
        }

        internal List<MethodDescription> Methods { get; set; }
    }
}
