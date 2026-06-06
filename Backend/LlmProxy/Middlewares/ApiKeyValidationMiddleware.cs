namespace LLM_Proxy_API.Middlewares
{
    public class ApiKeyValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiKeyValidationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Path.StartsWithSegments("/api/llm"))
            {
                await _next(context);
                return;
            }

            var expectedApiKey = _configuration["ServiceB:ApiKey"]
                ?? _configuration["SECRET_API_KEY"]
                ?? _configuration["ApiKey"];

            if (string.IsNullOrWhiteSpace(expectedApiKey))
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsJsonAsync(new { error = "Missing API key configuration." });
                return;
            }

            if (!context.Request.Headers.TryGetValue("X-API-KEY", out var providedApiKey) ||
                !string.Equals(providedApiKey.ToString(), expectedApiKey, StringComparison.Ordinal))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Unauthorized." });
                return;
            }

            await _next(context);
        }
    }
}
