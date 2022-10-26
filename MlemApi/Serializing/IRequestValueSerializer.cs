namespace MlemApi.Serializing
{
    public interface IRequestValueSerializer
    {
        string Serialize<T>(T value);
    }
}
