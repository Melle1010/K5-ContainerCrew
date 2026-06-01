namespace AI_Content_Assistant.DTOs
{
    /// <summary>
    /// Represents the request body for generating new AI content.
    /// </summary>
    public record LlmRequestDto(string Prompt);

    /// <summary>
    /// Represents the response returned for an AI content item.
    /// </summary>
    public record LlmResponseDto(string Answer);

}
