namespace MlemApi.Validation.Exceptions
{
    public class IllegalColumnsNumberException : Exception
    {
        public IllegalColumnsNumberException()
        {
        }

        public IllegalColumnsNumberException(string message)
            : base(message)
        {
        }

        public IllegalColumnsNumberException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
