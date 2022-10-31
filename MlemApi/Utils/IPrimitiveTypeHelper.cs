namespace MlemApi.Utils
{
    internal interface IPrimitiveTypeHelper
    {
        public string GetMappedDtype(string dType);

        public void ValidateType(string value, string expectedDtype);
    }
}
