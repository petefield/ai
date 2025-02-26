using Microsoft.Extensions.Hosting;
using OpenAI.Console.Services.Location;
using Microsoft.Extensions.DependencyInjection;
using OpenAI.Console.Tools;
using OpenAI.Console.Services.HomeAssistant;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHttpClient<LocationService>();
builder.Services.AddSingleton<ChatService>();
builder.Services.AddSingleton<WeatherService>();
builder.Services.AddSingleton<HomeAssistantService>();
builder.Services.AddSingleton<ToolBelt>();

using IHost host = builder.Build();


var chatService  = host.Services.GetRequiredService<ChatService>();

await chatService.Start();




