using AI_Content_Assistant.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AI_Content_Assistant.DTOs;

namespace CloudNativeInventory.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AiContentController : ControllerBase
{
    private readonly IAiContentService _service;

    public AiContentController(IAiContentService hfService)
    {
        _service = hfService;
    }

    /// <summary>
    /// Generates new AI content using the Gemini LLM via a proxy service.
    /// </summary>
    /// <param name="userQuery">The user prompt used to generate AI content.</param>
    /// <returns>The generated AI content.</returns>
    /// <response code="200">Content was generated successfully.</response>
    /// <response code="400">The request body was invalid or failed validation.</response>
    /// <response code="401">Authentication with the AI proxy failed.</response>
    /// <response code="429">The client exceeded the allowed request rate (rate limit).</response>
    /// <response code="502">The AI proxy returned an invalid response or the LLM backend failed.</response>
    /// <response code="503">The Gemini model is overloaded or temporarily unavailable.</response>
    /// <response code="504">The request to the AI proxy or LLM timed out.</response>
    /// <response code="500">An unexpected server error occurred.</response> 
    [HttpPost("generate/ai/posts")]
    public async Task<IActionResult> GenerateAsync([FromBody] string userQuery, CancellationToken ct)
    {
        var answer = await _service.CreateAsync(userQuery, ct);
        return Ok(new { answer });
    }

    /// <summary>
    /// Retrieves all available Gemini models currently available via a proxy service.
    /// </summary>
    [HttpGet("gemini/models")]
    public async Task<IActionResult> GetModels(CancellationToken ct)
    {
        var models = await _service.ListModelsAsync(ct);
        return Ok(models);
    }
}

