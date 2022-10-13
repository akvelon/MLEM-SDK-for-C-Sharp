namespace MlemApi.Dto
{
    internal class MethodDescription
    {
        public string MethodName { get; set; }

        public string ArgsName { get; set; }
        public IEnumerable<MethodArgumentData> ArgsData { get; set; }

        public MethodReturnData ReturnData {get; set;}
    }
}
