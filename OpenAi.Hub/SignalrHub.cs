using Microsoft.AspNetCore.SignalR;

namespace SignalRChat.Hubs
{
    public class OpenAIAssitantHub : Hub
    {

        public static string StateChangedEvent = nameof(StateChanged);
        public static string UserInputReceivedEvent = nameof(UserInputReceived);

        public async Task StateChanged(JeevesState state)
        {
            await Clients.All.SendAsync(StateChangedEvent, state);
        }

        public async Task UserInputReceived(string input)
        {
            await Clients.All.SendAsync(UserInputReceivedEvent, input);
        }
    }
}