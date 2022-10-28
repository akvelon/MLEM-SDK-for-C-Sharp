namespace MlemApi.Validation.Exceptions
{
    public class IllegalArrayNestingLevel : Exception
    {
        public IllegalArrayNestingLevel()
        {
        }

        public IllegalArrayNestingLevel(string message)
            : base(message)
        {
        }

        public IllegalArrayNestingLevel(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
