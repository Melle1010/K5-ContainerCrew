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
    string baseUrl;

    if (builder.Environment.IsDevelopment())
    {
        // Local development: Use localhost with HTTPS port (can be overridden via configuration)
        baseUrl = builder.Configuration["LlmProxy:BaseUrl"] ?? "https://localhost:7013/";
    }
    else
    {
        // Non-development (staging/production/container): prefer configured value, fall back to the Azure Container Apps internal hostname
        baseUrl = builder.Configuration["LlmProxy:BaseUrl"] 
            ?? "https://llmproxy-app.internal.ashyflower-20b74b17.swedencentral.azurecontainerapps.io/";
    }

    client.BaseAddress = new Uri(baseUrl);
});

//CORS POLICY
builder.Services.AddCors(options =>
{
    options.AddPolicy("StrictSecurityPolicy", policyBuilder =>
    {
        policyBuilder.WithOrigins("https://change-this-to-be-your-frontend-app.azurewebsites.net").WithMethods("GET", "POST").AllowAnyHeader(); //CHANGE THIS TO YOUR FRONTEND APP URL. THIS IS A PLACEHOLDER!!!
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

app.UseRateLimiter();

if (!app.Environment.IsEnvironment("Container"))
{
    app.UseHttpsRedirection();
}

app.UseCors("StrictSecurityPolicy");

app.MapControllers();

app.Run();