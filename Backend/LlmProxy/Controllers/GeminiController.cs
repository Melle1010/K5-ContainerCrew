using LLM_Proxy_API.Clients;
using LLM_Proxy_API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;

namespace LLM_Proxy_API.Controllers
{
    [ApiController]
    [EnableRateLimiting("sliding")]
    [Route("api/llm")]
    public class GeminiController : ControllerBase
    {
        private readonly GeminiClient _client;

        public GeminiController(GeminiClient client)
        {
            _client = client;
        }

        //[HttpPost("generate")]
        //public async Task<ActionResult<LlmResponseDto>> GenerateAsync(
        //    [FromBody] LlmRequestDto request,
        //    CancellationToken ct)
        //{
        //    var generatedText = await _client.GenerateAsync(request.Prompt, ct);

        //    var response = new LlmResponseDto(generatedText);

        //    return Ok(response);
        //}

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateAsync([FromBody] LlmRequestDto request, CancellationToken ct)
        {
            var result = await _client.GenerateAsync(request.Prompt, ct);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Raw);
            }

            // Parse the JSON from result.Raw
            var json = JsonDocument.Parse(result.Raw);
            var answer = json.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return Ok(new LlmResponseDto(answer ?? ""));

        }


        [HttpGet("models")]
        public async Task<string> GetModels(CancellationToken ct)
        {
            return await _client.ListModelsAsync(ct);
        }


    }
}
