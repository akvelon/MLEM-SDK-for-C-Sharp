using MlemApi.Parsing;

namespace MlemApi.Dto
{
    /// <summary>
    /// Interface for method description in mlem model api schema
    /// </summary>
    public class MethodDescription
    {
        public MethodDescription(string methodName, string argsName, ArgsData? argsData, IApiDescriptionDataStructure returnData)
        {
            MethodName = methodName;
            ArgsName = argsName;
            ArgsData = argsData;
            ReturnData = returnData;
        }

        public string MethodName { get; set; }

        public string? ArgsName { get; set; }
        public ArgsData? ArgsData { get; set; }

        public IApiDescriptionDataStructure ReturnData {get; set;}
    }
}
