using OpenAI.Chat;
using System.Reflection;
namespace OpenAI.Console;

public static class ExtensionMethods
{
    public static async Task<object> InvokeAsync(this MethodInfo @this, object obj, params object[] parameters)
    {
        var task = (Task)@this.Invoke(obj, parameters);
        await task.ConfigureAwait(false);
        var resultProperty = task.GetType().GetProperty("Result");
        return resultProperty.GetValue(task);
    }

    public static void AddRange(this IList<ChatTool> collection, IEnumerable<ChatTool> items)
    {
        foreach (var item in items)
        {
            collection.Add(item);
        }
    }

}
