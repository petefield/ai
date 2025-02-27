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
using OpenAI.Console.Services;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddJsonFile("appsettings.Development.json");
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddSingleton<LocationService>();
builder.Services.AddSingleton<ChatService>();
builder.Services.AddSingleton<WeatherService>();
builder.Services.AddSingleton<HomeAssistantService>();
builder.Services.AddSingleton<ToolBelt>();
builder.Services.AddSingleton<Voice>();
builder.Services.AddSingleton<SpeechRecognition>();

builder.Services.AddOptions<SpeechConfiguration>()
    .Bind(builder.Configuration.GetSection(nameof(SpeechConfiguration)));

builder.Services.AddOptions<OpenAIConfiguration>()
    .Bind(builder.Configuration.GetSection(nameof(OpenAIConfiguration)));

builder.Services.AddOptions<HomeAssistantServiceConfiguration>()
    .Bind(builder.Configuration.GetSection(nameof(HomeAssistantServiceConfiguration)));

builder.Services.AddOptions<WeatherServiceConfiguration>()
    .Bind(builder.Configuration.GetSection(nameof(WeatherServiceConfiguration)));


var host = builder.Build();


await host.RunService<ChatService>();



