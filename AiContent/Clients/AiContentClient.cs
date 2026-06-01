using AI_Content_Assistant.DTOs;
using System.Net.Http.Json;

namespace AI_Content_Assistant.Clients
{
    public class AiContentClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AiContentClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<HttpResponseMessage> SendPromptAsync(LlmRequestDto dto, CancellationToken ct)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "api/llm/generate")
            {
                Content = JsonContent.Create(dto)
            };
            var apiKey = _config["ServiceB:ApiKey"];
            request.Headers.Add("X-API-KEY", apiKey);

            return await _httpClient.SendAsync(request, ct);
        }

        public async Task<string> GetModelsAsync(CancellationToken ct)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "api/llm/models");

            var apiKey = _config["ServiceB:ApiKey"];
            request.Headers.Add("X-API-KEY", apiKey);

            var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync(ct);
        }


    }
}
