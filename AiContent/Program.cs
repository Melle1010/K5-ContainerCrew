using AI_Content_Assistant.Clients;
using AI_Content_Assistant.Exceptions;
using AI_Content_Assistant.Extensions;
using AI_Content_Assistant.Filters;
using AI_Content_Assistant.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Reflection;



var builder = WebApplication.CreateBuilder(args);

// Controllers + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);
});


// Services
builder.Services.AddScoped<IAiContentService, AiContentService>();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExecutionTimeFilter>();
});

//Ratelimiting
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (context, token) =>
    {
        // Throwing here will produce a faulted Task and bubble to your ExceptionMiddleware
        throw new RateLimitException("You have exceeded the allowed number of requests. Please try again later.");
    };

    options.AddSlidingWindowLimiter("sliding", config =>
    {
        config.Window = TimeSpan.FromMinutes(1);
        config.SegmentsPerWindow = 2;
        config.PermitLimit = 2;
    });
});


builder.Services.AddHttpClient<AiContentClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:7013/"); // Service B HTTPS port
});

var app = builder.Build();



app.UseCustomExceptionHandling();

// Swagger
//app.UseSwagger();
//app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    // Generate OpenAPI JSON
    app.UseSwagger(options =>
    {
        options.RouteTemplate = "openapi/{documentName}.json";
    });

    // Enable Scalar UI
    app.MapScalarApiReference();
}

app.UseRateLimiter();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
