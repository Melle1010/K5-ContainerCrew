using AI_Content_Assistant.Clients;
using AI_Content_Assistant.Exceptions;
using AI_Content_Assistant.Extensions;
using AI_Content_Assistant.Filters;
using AI_Content_Assistant.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Reflection;
using Azure.Monitor.OpenTelemetry.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//Activating Teleometry which sends data to Azure Monitor
//builder.Services.AddOpenTelemetry().UseAzureMonitor();

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

// HttpClient Configuration with Base URL and Key Validation Injection
builder.Services.AddHttpClient<AiContentClient>(client =>
{
    string? baseUrl = builder.Configuration["LlmProxy:BaseUrl"];

    // Om variabeln saknas helt i produktion/container slänger vi ett fel i stället för att ha en hårdkodad länk
    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        if (builder.Environment.IsDevelopment())
        {
            baseUrl = "https://localhost:7013/";
        }
        else
        {
            throw new InvalidOperationException("Missing critical configuration: 'LlmProxy:BaseUrl' is not set in the environment.");
        }
    }

    client.BaseAddress = new Uri(baseUrl);

    var apiKey = builder.Configuration["ServiceB:ApiKey"]
        ?? Environment.GetEnvironmentVariable("ServiceB__ApiKey");

    if (!string.IsNullOrWhiteSpace(apiKey))
    {
        client.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
    }
});

// CORS POLICY
builder.Services.AddCors(options =>
{
    options.AddPolicy("StrictSecurityPolicy", policyBuilder =>
    {
        // Hämtar tillåtna origins från appsettings.json eller Azure, faller tillbaka på localhost under utveckling
        var allowedOrigin = builder.Configuration["AllowedOrigins"] ?? "http://localhost:5173";

        policyBuilder
            .WithOrigins(allowedOrigin)
            .WithMethods("GET", "POST")
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCustomExceptionHandling();

// Swagger and Scalar UI
// Generate OpenAPI JSON
app.UseSwagger(options =>
{
    options.RouteTemplate = "openapi/{documentName}.json";
});

// Enable Scalar UI
app.MapScalarApiReference();

//app.UseRateLimiter();

if (!app.Environment.IsEnvironment("Container"))
{
    app.UseHttpsRedirection();
}

app.UseCors("StrictSecurityPolicy");

app.MapControllers();

app.Run();