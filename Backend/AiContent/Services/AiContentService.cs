using AI_Content_Assistant.Clients;
using AI_Content_Assistant.DTOs;
using AI_Content_Assistant.Exceptions;
using AI_Content_Assistant.Validators;
using System.Diagnostics;
using System.Net;

namespace AI_Content_Assistant.Services
{
    public class AiContentService : IAiContentService
    {
        private readonly AiContentClient _client;
        private readonly ILogger<AiContentService> _logger;
        private readonly IConfiguration _config;

        public AiContentService(
            AiContentClient client,
            ILogger<AiContentService> logger,
            IConfiguration config)
        {
            _client = client;
            _logger = logger;
            _config = config;
        }

        public async Task<string> ListModelsAsync(CancellationToken ct)
        {
            return await _client.GetModelsAsync(ct);
        }

        public async Task<string> CreateAsync(string userQuery, CancellationToken ct)
        {
            // Stopwatch for per-step timing
            var stepTimer = Stopwatch.StartNew();

            LogStep("Building a Gemini prompt...", stepTimer);

            await Task.Delay(10);

            // 1. Building system + user message
            string systemMessage =
                "Your name is Bob." +
                "You will be speaking to clients that will be using our digital timer application, therefore you must be helpful and respectful. You find the meaning of life with helping people." +
                "You are an expert in timer and clock since you have worked with it all your life. You could easily tell everything about the clock just by hearing its ticking sound" +
                "Make your answer brief, maximum 5 sentences. Each sentence should not be too long also, maximum 20 words.";

            string finalPrompt =
                $"System: {systemMessage}\nUser: {userQuery}\nAssistant:";

            // 2. Creating DTO to send to Service B
            var requestDto = new LlmRequestDto(finalPrompt);

            LogStep("Sending prompt to Service B...", stepTimer);

            // 3. Sending DTO to Service B
            var response = await _client.SendPromptAsync(requestDto, ct);

            LogStep($"Response received from Service B status {response.StatusCode}", stepTimer);

            // 4. Error handling
            var status = (int)response.StatusCode;

            //switch (status)
            //{
            //    case 401:
            //    case 403:
            //        throw new UnauthorizedAccessException("Client is unauthorized or unauthenticated.");

            //    case 429:
            //        throw new RateLimitException("Rate limit exceeded. Please retry later.");

            //    case int s when s >= 500 && s < 600:
            //        throw new AiExternalException($"Service B returned {status}.");

            //    default:
            //        if (!response.IsSuccessStatusCode)
            //            throw new AiExternalException($"Service B returned {status}.");
            //        break;
            //}


            // Any non-success from Service B becomes an LlmProxyException
            if (!response.IsSuccessStatusCode)
            {
                throw new LlmProxyException(
                    status,
                    $"Service B returned status {status}."
                );
            }


            // 5. Deserialize LlmResponseDto
            var dto = await response.Content.ReadFromJsonAsync<LlmResponseDto>(ct);

            if (dto == null || string.IsNullOrWhiteSpace(dto.Answer))
            {
                throw new AiEmptyResponseException("Service B returned an empty response.");
            }

            // 6 Validate quality of the AI content
            AiContentValidator.Validate(dto.Answer);

            LogStep("Gemini successfully generated output.", stepTimer);

            // 7. Return final answer
            return dto.Answer;
        }

        private void LogStep(string message, Stopwatch timer)
        {
            var elapsed = timer.ElapsedMilliseconds;

            _logger.LogInformation($"LOG: {message} (Elapsed={elapsed}ms)");

            timer.Restart();
        }
    }
}
