using LLM_Proxy_API.DTOs;
using System.Net.Http.Json;
using System.Text.Json;

namespace LLM_Proxy_API.Clients
{
    public class GeminiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public GeminiClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        //public async Task<string> GenerateAsync(string prompt, CancellationToken ct = default)
        public async Task<(bool Success, int StatusCode, string Raw)> GenerateAsync(string prompt, CancellationToken ct)

        {
            var apiKey = _config["Gemini:ApiKey"]
                ?? throw new Exception("Missing Gemini API key");

            // Gemini request format
            var requestBody = new
            {
                contents = new[]
                {
                    new
                    {
                        parts = new[]
                        {
                            new { text = prompt }
                        }
                    }
                }
            };

            var request = new HttpRequestMessage(
                HttpMethod.Post,
                $"v1/models/gemini-2.5-flash:generateContent?key={apiKey}"
            )
            {
                Content = JsonContent.Create(requestBody)
            };

            var httpResponse = await _httpClient.SendAsync(request, ct);
            //httpResponse.EnsureSuccessStatusCode();
            var raw = await httpResponse.Content.ReadAsStringAsync(ct);
            Console.WriteLine(raw);

            //if (!httpResponse.IsSuccessStatusCode)
            //{
            //    throw new Exception($"Gemini error {(int)httpResponse.StatusCode}: {raw}");
            //}


            //var json = await httpResponse.Content.ReadFromJsonAsync<JsonElement>(cancellationToken: ct);

            //// Extract the text from Gemini response
            //var answer =
            //    json.GetProperty("candidates")[0]
            //        .GetProperty("content")
            //        .GetProperty("parts")[0]
            //        .GetProperty("text")
            //        .GetString();

            //return answer ?? "";
            return (httpResponse.IsSuccessStatusCode, (int)httpResponse.StatusCode, raw);
        }

        public async Task<string> ListModelsAsync(CancellationToken ct = default)
        {
            var apiKey = _config["Gemini:ApiKey"]
                ?? throw new Exception("Missing Gemini API key");

            var response = await _httpClient.GetAsync(
                $"v1/models?key={apiKey}",
                ct
            );

            var raw = await response.Content.ReadAsStringAsync(ct);
            return raw;
        }
    }
}
