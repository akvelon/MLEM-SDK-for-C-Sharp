namespace MlemApi.Validation.Exceptions
{
    public class IllegalArrayNestingLevelException : Exception
    {
        public IllegalArrayNestingLevelException()
        {
        }

        public IllegalArrayNestingLevelException(string message)
            : base(message)
        {
        }

        public IllegalArrayNestingLevelException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
