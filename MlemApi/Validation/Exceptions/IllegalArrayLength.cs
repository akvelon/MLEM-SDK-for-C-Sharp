namespace MlemApi.Validation.Exceptions
{
    public class IllegalArrayLength : Exception
    {
        public IllegalArrayLength()
        {
        }

        public IllegalArrayLength(string message)
            : base(message)
        {
        }

        public IllegalArrayLength(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
