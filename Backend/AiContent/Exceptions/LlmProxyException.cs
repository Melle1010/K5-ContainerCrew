namespace AI_Content_Assistant.Exceptions
{
    public class LlmProxyException : Exception
    {
        public int StatusCode { get; }

        public LlmProxyException(int statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }
}

