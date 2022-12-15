namespace MlemApi.Dto
{
    public class ApiDescription
    {
        public ApiDescription()
        {
            Methods = new List<MethodDescription>();
        }

        public List<MethodDescription> Methods { get; set; }
    }
}
