using AI_Content_Assistant.Clients;
using AI_Content_Assistant.DTOs;
using AI_Content_Assistant.Exceptions;
using AI_Content_Assistant.Validators;
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
            _logger.LogInformation($"LOG: Building a Gemini prompt...");

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

            _logger.LogInformation($"LOG: Sending prompt to Service B...");

            // 3. Sending DTO to Service B
            var response = await _client.SendPromptAsync(requestDto, ct);

            _logger.LogInformation($"LOG: Response received from Service B statis {StatusCode}", response.StatusCode);

            // 4. Error handling
            var status = (int)response.StatusCode;

            switch (status)
            {
                case 401:
                case 403:
                    throw new UnauthorizedAccessException("Client is unauthorized or unauthenticated.");

                case 429:
                    throw new RateLimitException("Rate limit exceeded. Please retry later.");

                case 503:
                    throw new GeminiOverloadedException("Gemini is currently overloaded. Please try again later.");

                case >= 500 and < 600:
                    throw new AiExternalException($"Service B returned {status}.");

                default:
                    if (!response.IsSuccessStatusCode)
                        throw new AiExternalException($"Service B returned {status}.");
                    break;
            }


            // 5. Deserialize LlmResponseDto
            var dto = await response.Content.ReadFromJsonAsync<LlmResponseDto>(ct);

            if (dto == null || string.IsNullOrWhiteSpace(dto.Answer))
            {
                throw new AiEmptyResponseException("Service B returned an empty response.");
            }

            _logger.LogInformation($"LOG: Gemini successfully generated output.");

            // 6 Validate quality of the AI content
            AiContentValidator.Validate(dto.Answer);

            // 7. Return final answer
            return dto.Answer;
        }
    }
}
