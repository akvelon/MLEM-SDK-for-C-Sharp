namespace MlemApi.Dto
{
    internal class ApiDescription
    {
        internal ApiDescription()
        {
            Methods = new List<MethodDescription>();
        }

        internal List<MethodDescription> Methods { get; set; }
    }
}
