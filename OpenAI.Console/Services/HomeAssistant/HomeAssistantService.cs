using Newtonsoft.Json;
using OpenAI.Console.Tools;
using Simple.HAApi;
using Simple.HAApi.Sources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAI.Console.Services.HomeAssistant
{
    internal class HomeAssistantService
    {
        private readonly Instance instance;

        public HomeAssistantService()
        {
            instance = new Instance(new Uri("http://pippin:8123"), "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJkZjQxNjM1ZDZiMjE0ZGViODYzYjMyZjQ1ODgwZDIwYyIsImlhdCI6MTc0MDQxMTg1MiwiZXhwIjoyMDU1NzcxODUyfQ.pQip37QkKCEc3WZnVwvV7HYZ2OwGWRoh-yvrj2O3LsY");
     
            instance.IgnoreCertificatErrors = true;
        }

        public async Task<string> GetConfig() { 
        
       
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
            var r = all.Select(x => new { 
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
}
