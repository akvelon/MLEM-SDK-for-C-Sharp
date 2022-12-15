namespace MlemApi.Dto
{
    public class MethodDescription
    {
        public MethodDescription(string methodName, string argsName, IApiDescriptionDataStructure argsData, NdarrayData returnData)
        {
            MethodName = methodName;
            ArgsName = argsName;
            ArgsData = argsData;
            ReturnData = returnData;
        }

        public string MethodName { get; set; }

        public string? ArgsName { get; set; }
        public IApiDescriptionDataStructure? ArgsData { get; set; }

        public IApiDescriptionDataStructure ReturnData {get; set;}
    }
}
