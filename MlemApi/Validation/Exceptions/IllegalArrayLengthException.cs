namespace MlemApi.Validation.Exceptions
{
    public class IllegalArrayLengthException : Exception
    {
        public IllegalArrayLengthException()
        {
        }

        public IllegalArrayLengthException(string message)
            : base(message)
        {
        }

        public IllegalArrayLengthException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
