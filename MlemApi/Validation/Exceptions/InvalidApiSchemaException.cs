namespace MlemApi.Validation.Exceptions
{
    public class InvalidApiSchemaException : Exception
    {
        public InvalidApiSchemaException()
        {
        }

        public InvalidApiSchemaException(string message)
            : base(message)
        {
        }

        public InvalidApiSchemaException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
