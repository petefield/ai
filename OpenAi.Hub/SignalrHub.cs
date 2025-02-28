using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class OpenAIAssitantHub : Hub
    {
        public async Task StateChanged(string state)
        {
            await Clients.All.SendAsync("StateChanged", state);
        }
    }
}