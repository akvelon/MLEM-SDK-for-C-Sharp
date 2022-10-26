namespace MlemApi.Dto
{
    internal class MethodDescription
    {
        public string MethodName { get; set; }

        public string ArgsName { get; set; }
        public IMethodArgumentData ArgsData { get; set; }

        public NdarrayData ReturnData {get; set;}
    }
}
