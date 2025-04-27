using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Console;
using OpenAI.Console.Configuration;
using OpenAI.Console.Tools;
using OpenAI.Console.Services.HomeAssistant;
using OpenAI.Console.Services.Location;
using OpenAI.Console.Services.Weather;
using SpeechToText;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddSingleton<LocationService>();
builder.Services.AddSingleton<ChatService>();
builder.Services.AddSingleton<WeatherService>();
builder.Services.AddSingleton<HomeAssistantService>();
builder.Services.AddSingleton<ToolBelt>();
builder.Services.AddSingleton<Voice>();

builder.AddConfiguration<OpenAIConfiguration>();
builder.AddConfiguration<SpeechConfiguration>();
builder.AddConfiguration<HomeAssistantServiceConfiguration>();
builder.AddConfiguration<WeatherServiceConfiguration>();

var host = builder.Build();

await host.RunService<ChatService>();



