using MlemApi.Dto;
using MlemApi.Serializing;

namespace MlemApi.Parsing
{
    public class ArgsData : IApiDescriptionDataStructure
    {
        public IApiDescriptionDataStructure? DataType { get; set; }
        public IRequestValuesSerializer? Serializer { get; set; }
    }
}
