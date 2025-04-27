using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Jeeves.VoiceRecognition;

public static class ExtensionMethods
{
    public static Task RunService<TService>(this IHost host) where TService : IStart
        => host.Services.GetRequiredService<TService>().Start();
}
