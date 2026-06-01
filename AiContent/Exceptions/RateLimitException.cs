namespace AI_Content_Assistant.Exceptions
{
    public class RateLimitException : Exception
    {
        public RateLimitException(string message) : base(message) { }
    }
}

