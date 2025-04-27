
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.CognitiveServices.Speech;
using SignalRChat.Hubs;

namespace Jeeves.VoiceRecognition
{
    internal class Listener(SpeechRecognition speechRecognition) : IStart
    {
        private HubConnection _connection;

        public async Task Start()
        {
            var connectionAttempts = 0;
            _connection = new HubConnectionBuilder()
                .WithUrl("http://localhost:5057/OpenAIAssitantHub")
                .Build();

            while (connectionAttempts <= 50)
            {
                try
                {
                    await _connection.StartAsync();
                    break;
                }
                catch (Exception)
                {
                }
                await Task.Delay(500);
                connectionAttempts++;
            }

            speechRecognition.OnSpeechRecognised = async (s, e) =>
            {
                Console.Write(e);
                await _connection.InvokeAsync("UserInputReceived", e);
            };

            speechRecognition.OnKeyWordRecognised = async (s, e) =>
            {
                Console.Write("Keyword heard.");
                await speechRecognition.StartListening();
            };

            speechRecognition.OnStateChanged = (s, e) => Console.Write(e);

            _connection.On<JeevesState>(OpenAIAssitantHub.StateChangedEvent, async ( state) => {
                switch (state)
                {
                    case  JeevesState.Speaking:
                        await speechRecognition.PauseListening();
                        break;
                    case JeevesState.Listening:
                        await speechRecognition.StartListening();
                        break;
                }
            });

            await speechRecognition.StartListeningForKeyWord();
            Console.ReadLine();
        }
    }
}
