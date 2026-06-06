using LLM_Proxy_API.Clients;
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


var app = builder.Build();

//app.UseApiKeyValidation();

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


app.UseHttpsRedirection();

app.MapControllers();

app.Run();
