
using Newtonsoft.Json;
using OpenAI.Chat;
using System.Reflection;
using System.Text;
using System.Text.Json;

using OpenAI.Console.Services.Location;
using OpenAI.Console.Services.HomeAssistant;
namespace OpenAI.Console.Tools;

internal partial class ToolBelt(LocationService locationService, WeatherService WeatherService, HomeAssistantService homeAssistantService)
{
    public static IEnumerable<ChatTool> GetTools()
    {

        var methods = typeof(ToolBelt).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
              .Where(m => m.GetCustomAttributes(typeof(ToolAttribute), false).Length > 0);


        foreach (var method in methods)
        {

            ToolParametersDescriptor f = new ToolParametersDescriptor();

            foreach (var parameter in method.GetParameters())
            {
                var parameterAttribute = parameter.GetCustomAttribute<ToolParameterAttribute>();
                f.properties.Add(parameter.Name, new ToolParameterDescriptor(parameter.ParameterType.Name.ToLower(), parameterAttribute.Description));
            }

            var methodAttribute = method.GetCustomAttribute<ToolAttribute>();

            var s = JsonConvert.SerializeObject(f);

            yield return ChatTool.CreateFunctionTool(
                functionName: method.Name,
                functionDescription: methodAttribute.Description,
                functionParameters: BinaryData.FromBytes(Encoding.UTF8.GetBytes(s)));
        }

    }

    public async Task<string> CallTool(ChatToolCall toolCall)
    {
        try
        {
            var methodName = toolCall.FunctionName;
            var s = toolCall.FunctionArguments;

            using JsonDocument argumentsJson = JsonDocument.Parse(toolCall.FunctionArguments);
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(argumentsJson.RootElement.ToString());

            var t = typeof(ToolBelt);

            var method = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
               .Where(m => m.GetCustomAttributes(typeof(ToolAttribute), false).Length > 0)
               .Where(m => m.Name == methodName).Single();

            var parameters = method.GetParameters().Select(p => dict.TryGetValue(p.Name, out object parameterValue)
                    ? Convert.ChangeType(parameterValue, p.ParameterType)
                    : null
        );

            var result = await method.InvokeAsync(this, parameters.ToArray());
            return result?.ToString() ?? "Done.";

        }
        catch (Exception ex)
        {
            return $"Failed with message {ex.Message}";

            throw ex;
        }
    }

    [Tool("Get the user's current location")]
    public async Task<string> GetCurrentLocation()
        => (await locationService.GetLocationData()).ToString();

   
    [Tool("Get the user's current location")]
    public async Task<string> GetCurrentWeather(
        [ToolParameter("The latitude for which the current weather needs to be determined")] string latitude,
        [ToolParameter("The longditude for which the current weather needs to be determined.")] string longditude)
        => (await WeatherService.GetWeatherData(latitude, longditude)).ToString();

    [Tool("Calls a home automation service")]
    public  async Task<string> CallHomeAutomationService(
        [ToolParameter("The id of the entity on which to perform the service. Entities can be determined by calling the 'GetHomeStates' tool. This entity id should include the domain")] string entity,
        [ToolParameter("The Domain to which the entity belongs ")] string entityDomain,
        [ToolParameter("The name of the service to call. Service names can be determined by calling the 'GetHomeServices' tool ")] string serviceName)
        => await homeAssistantService.CallService(serviceName, entityDomain, entity);

    [Tool("Gets the current date and time in UTC timezone")]
    public static Task<string> GetTime()
        => Task.FromResult(DateTime.UtcNow.ToString());

    [Tool("Gets home automation config")]
    public async Task<string> GetHomeConfig()
       => await homeAssistantService.GetConfig();

    [Tool("Gets home automation services")]
    public async Task<string> GetHomeServices()
        => await homeAssistantService.GetServices();

    [Tool("Gets all home automation entity states")]
    public async Task<string> GetHomeStates()
    => await homeAssistantService.GetStates();

}
