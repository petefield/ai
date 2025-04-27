using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenAI.Chat;
using System.Reflection;

namespace OpenAI.Console;

public static class ExtensionMethods
{
    public static void AddConfiguration<T>(this HostApplicationBuilder builder, string? path = null) where T : class
    {
        var configPath = path ?? typeof(T).Name;
        builder.Services.AddOptions<T>()
            .Bind(builder.Configuration.GetSection(configPath));
    }


    public static async Task<object?> InvokeAsync(this MethodInfo @this, object obj, params object?[] parameters)
    {
        var task = (Task?)@this.Invoke(obj, parameters);

        if (task is null)
            return Task.FromResult<object?>(null);

        await task.ConfigureAwait(false);

        var resultProperty = task.GetType().GetProperty("Result");

        return resultProperty?.GetValue(task);
    }

    public static void AddRange(this IList<ChatTool> collection, IEnumerable<ChatTool> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

    public static Task RunService<TService>(this IHost host) where TService : IStart
        => host.Services.GetRequiredService<TService>().Start();
}

public interface IStart
{
    Task Start();
}