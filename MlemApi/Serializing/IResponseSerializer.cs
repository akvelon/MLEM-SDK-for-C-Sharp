namespace MlemApi.Serializing
{
    public interface IResponseSerializer
    {
       T Serialize<T>(HttpResponseMessage response);
    }
}
