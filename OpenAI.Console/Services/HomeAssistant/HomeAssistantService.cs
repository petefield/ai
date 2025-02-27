using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using OpenAI.Console.Configuration;
using Simple.HAApi;

namespace OpenAI.Console.Services.HomeAssistant;

internal class HomeAssistantService
{
    private readonly Instance instance;

    public HomeAssistantService(IOptions<HomeAssistantServiceConfiguration> homeAssistantServiceConfiguration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(homeAssistantServiceConfiguration.Value.Endpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(homeAssistantServiceConfiguration.Value.Key);

        instance = new Instance(new Uri(homeAssistantServiceConfiguration.Value.Endpoint), homeAssistantServiceConfiguration.Value.Key);

        instance.IgnoreCertificatErrors = true;
    }

    public async Task<string> GetConfig()
    {
        // Get a source
        var cfgSource = instance.Get<Simple.HAApi.Sources.Configuration>();
        // Get info as needed
        var config = await cfgSource.GetConfigurationAsync();
        var entries = await cfgSource.GetConfigurationEntriesAsync();

        var s = JsonConvert.SerializeObject(entries);

        return s;
    }

    public async Task<string> GetServices()
    {
        var srvSource = instance.Get<Simple.HAApi.Sources.Service>();
        // Get all services
        var services = await srvSource.GetServicesAsync();
        var s = JsonConvert.SerializeObject(services);
        return s;
    }

    public async Task<string> GetStates()
    {
        // Get a source
        var statesSource = instance.Get<Simple.HAApi.Sources.States>();
        var all = await statesSource.GetStatesAsync();
        var r = all.Select(x => new
        {
            x.Domain,
            x.EntityId,
            x.State,
            x.FriendlyName,
        }).ToArray();
        var s = JsonConvert.SerializeObject(r);
        return s;
    }

    public async Task<string> CallService(string service, string domain, string entityid)
    {
        var srvSource = instance.Get<Simple.HAApi.Sources.Service>();

        // Or call a service for a single entity
        var x = await srvSource.CallServiceAsync($"{domain}.{service}", entityid);
        // or multiple
        var s = JsonConvert.SerializeObject(x);
        return s;
    }

}
