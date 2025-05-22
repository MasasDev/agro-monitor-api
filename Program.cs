using AgroMonitor.Data;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using System.Buffers.Text;
using AgroMonitor.Services.Interfaces;
using AgroMonitor.Shared;
using AgroMonitor.Services;
using AgroMonitor.DTOs;
using AgroMonitor.Controllers;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
string connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")!;

string geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY")!;

string? geminiBaseUrl = $"{builder.Configuration["ApiSettings:Gemini_API_Url"]}";

builder.Services.AddHttpClient<IGeminiService, GeminiService>(client =>
{
    client.BaseAddress = new Uri($"{geminiBaseUrl}?key={geminiApiKey}");
});

builder.Services.AddScoped<ISensorReadingsProcessor, SensorReadingsProcessor>();

builder.Services.AddScoped<ITwilioService, TwilioService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
