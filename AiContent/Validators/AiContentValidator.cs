using AI_Content_Assistant.Exceptions;

namespace AI_Content_Assistant.Validators
{
    public static class AiContentValidator
    {
        public static void Validate(string aiText)
        {
            if (string.IsNullOrWhiteSpace(aiText))
                throw new AiContentQualityException("AI response is empty or invalid.");

            int sentenceCount = 0;

            foreach (char c in aiText)
            {
                if (c == '.' || c == '!' || c == '?')
                {
                    sentenceCount++;
                }
            }

            if (sentenceCount > 5)
                throw new AiContentQualityException(
                    $"AI response contains too many sentences ({sentenceCount}). Maximum allowed is 5.");
        }
    }
}
