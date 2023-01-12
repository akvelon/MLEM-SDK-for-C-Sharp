using MlemApi.Dto;
using MlemApi.Serializing;

namespace MlemApi.Parsing
{
    public class ReturnData : IApiDescriptionDataStructure
    {
        public IApiDescriptionDataStructure? DataType { get; set; }
        public IResponseSerializer Serializer { get; set; }
    }
}
