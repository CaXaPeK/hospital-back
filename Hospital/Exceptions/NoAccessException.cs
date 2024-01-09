namespace Hospital.Exceptions
{
    public class NoAccessException : Exception
    {
        public NoAccessException()
        {
        }

        public NoAccessException(string message) : base(message)
        {
        }

        public NoAccessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
