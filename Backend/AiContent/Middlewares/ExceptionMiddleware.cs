namespace AI_Content_Assistant.Middleware
{
    using AI_Content_Assistant.Exceptions;
    using Microsoft.AspNetCore.Mvc;

    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred");

                var problem = MapToProblemDetails(ex);

                context.Response.StatusCode = problem.Status ?? 500;
                context.Response.ContentType = "application/problem+json";

                await context.Response.WriteAsJsonAsync(problem);
            }
        }

        private ProblemDetails MapToProblemDetails(Exception ex)
        {
            return ex switch
            {
                ContentNotFoundException nf => new ProblemDetails
                {
                    Status = 404,
                    Title = "Not Found",
                    Detail = nf.Message
                },

                BadRequestException br => new ProblemDetails
                {
                    Status = 400,
                    Title = "Bad Request",
                    Detail = br.Message
                },

                UnauthorizedAccessException ua => new ProblemDetails
                {
                    Status = 401,
                    Title = "Unauthorized",
                    Detail = ua.Message
                },

                RateLimitException rl => new ProblemDetails
                {
                    Status = 429,
                    Title = "Too Many Requests",
                    Detail = rl.Message
                },

                AiExternalException or AiEmptyResponseException or HttpRequestException => new ProblemDetails
                {
                    Status = 502,
                    Title = "Bad Gateway",
                    Detail = ex.Message
                },

                AiContentQualityException aq => new ProblemDetails
                {
                    Status = 502,
                    Title = "AI Content Quality Error",
                    Detail = "The server received an invalid response from the LLM, please try again later."
                },

                GeminiOverloadedException go => new ProblemDetails
                {
                    Status = 503,
                    Title = "Service Unavailable",
                    Detail = go.Message
                },

                TaskCanceledException or OperationCanceledException => new ProblemDetails
                {
                    Status = 504,
                    Title = "Gateway Timeout",
                    Detail = "The external LLM service did not respond in time."
                },

                _ => new ProblemDetails
                {
                    Status = 500,
                    Title = "Internal Server Error",
                    Detail = "An unexpected error occurred."
                }
            };

        }
    }
}
