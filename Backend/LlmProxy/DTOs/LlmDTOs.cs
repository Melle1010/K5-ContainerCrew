namespace LLM_Proxy_API.DTOs
{
    public record LlmRequestDto(string Prompt);

    public record LlmResponseDto(string Answer);

}
