// See https://aka.ms/new-console-template for more information
using Jeeves.VoiceRecognition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Console.WriteLine("Hello, World!");
HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddUserSecrets<Program>();

builder.Services.AddSingleton<SpeechRecognition>();
builder.Services.AddSingleton<Listener>();
builder.Services.AddOptions<SpeechConfiguration>()
    .Bind(builder.Configuration.GetSection(nameof(SpeechConfiguration)));

var host = builder.Build();

await host.RunService<Listener>();
