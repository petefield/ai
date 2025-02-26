using Microsoft.Extensions.Hosting;
using OpenAI.Console.Services.Location;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Console.Tools;
using OpenAI.Console.Services.HomeAssistant;
using SpeechToText;
using Microsoft.Extensions.Configuration;
using OpenAI.Console.Configuration;
using OpenAI.Console;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddHttpClient<LocationService>();
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

using IHost host = builder.Build();

var chatService  = host.Services.GetRequiredService<ChatService>();

await chatService.Start();




