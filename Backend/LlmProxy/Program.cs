using LLM_Proxy_API.Clients;
using LLM_Proxy_API.Middlewares;
using Microsoft.AspNetCore.RateLimiting;
//using LLM_Proxy_API.Extensions;
//using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<GeminiClient>(client =>
{
    client.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
});

// Ratelimiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsJsonAsync(new
        {
            status = 429,
            title = "Too Many Requests",
            detail = "You have exceeded the allowed number of requests. Please try again later."
        }, cancellationToken: token);
    };


    options.AddSlidingWindowLimiter("sliding", config =>
    {
        config.Window = TimeSpan.FromMinutes(1);
        config.SegmentsPerWindow = 2;
        config.PermitLimit = 2;
    });
});


var app = builder.Build();

app.UseMiddleware<ApiKeyValidationMiddleware>();

// Swagger
//app.UseSwagger();
//app.UseSwaggerUI();

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger(options =>
//    {
//        options.RouteTemplate = "openapi/{documentName}.json";
//    });

//    app.MapScalarApiReference();
//}


if (!app.Environment.IsEnvironment("Container"))
{
    app.UseHttpsRedirection();
}

app.MapControllers();

app.Run();
