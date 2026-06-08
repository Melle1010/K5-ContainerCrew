namespace AI_Content_Assistant.Exceptions
{
    public class AiEmptyResponseException : Exception
    {
        public AiEmptyResponseException(string message) : base(message) { }
    }
}
