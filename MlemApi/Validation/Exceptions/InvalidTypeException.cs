namespace MlemApi.Validation.Exceptions
{
    public class InvalidTypeException : Exception
    {
        public InvalidTypeException()
        {
        }

        public InvalidTypeException(string message)
            : base(message)
        {
        }

        public InvalidTypeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
