namespace MlemApi.Validation.Exceptions
{
    public class IllegalPathException : Exception
    {
        public IllegalPathException()
        {
        }

        public IllegalPathException(string message)
            : base(message)
        {
        }

        public IllegalPathException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
