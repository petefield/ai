using OpenAI.Chat;
using System.ClientModel;
using OpenAI.Console.Tools;
using Microsoft.Extensions.Options;
using OpenAI.Console;
using SpeechToText;
using OpenAI.Console.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.CognitiveServices.Speech;

internal class ChatService : IStart
{
    private readonly ToolBelt _toolBelt;
    private readonly Voice _voice;
    private readonly SpeechRecognition _speechRecognition;
    private readonly List<ChatMessage> _messages;
    private readonly ChatClient? _client = null;
    private readonly ChatCompletionOptions _chatCompletionOptions = new();
    private readonly HubConnection _connection;

    public ChatService(ToolBelt toolBelt,
        Voice voice,
        SpeechRecognition speechRecognition,
        IOptions<OpenAIConfiguration> openAIConfiguration)
    {
        ArgumentNullException.ThrowIfNull(openAIConfiguration.Value.Key);

        _toolBelt = toolBelt;
        _voice = voice;
        _speechRecognition = speechRecognition;

        _client = new(
            model: openAIConfiguration.Value.Model,
            credential: new ApiKeyCredential(openAIConfiguration.Value.Key)
        );

        var sp = File.ReadAllText("SystemPrompt.txt");

        _messages = [new SystemChatMessage(sp)];

        _chatCompletionOptions.Tools.AddRange(toolBelt.GetTools());

        _speechRecognition.OnSpeechRecognised = async (s, e) =>
        {
            await _speechRecognition.PauseListening();
            Console.Write(e);
            await Chat(e);
        };

        _speechRecognition.OnKeyWordRecognised = async (s, e) => await DisplayAssistantMessage("Yes Sir?");

        _speechRecognition.OnStateChanged = async (s, e) => await DisplayState(e);

        _connection = new HubConnectionBuilder()
               .WithUrl("http://localhost:5057/OpenAIAssitantHub")
               .Build();
    }

    public async Task Start()
    {
        var connectionAttempts = 0;

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


        Console.WriteLine("Chat Service Started");
        var welcomeMessage = "Hello Sir. If you need me, simply say 'Computer'.";

        await DisplayAssistantMessage(welcomeMessage, false);

        await _speechRecognition.StartListeningForKeyWord();

        while (true)
        {
            await Chat(Console.ReadLine());
        }
    }
    
    private async Task Chat(string? userInput)
    {
        if (string.IsNullOrEmpty(userInput))
            return;

        if (string.Equals(userInput, "stop listening.", StringComparison.OrdinalIgnoreCase))
        {
            await DisplayAssistantMessage("Certainly Sir.", false);
            await _speechRecognition.StartListeningForKeyWord();
            return;
        }

        _messages.Add(new UserChatMessage(userInput));

        ChatCompletion completion;

        await DisplayState("Working");

        do
        {
            completion = _client!.CompleteChat(_messages, _chatCompletionOptions);

            switch (completion.FinishReason)
            {
                case ChatFinishReason.Stop:
                    _messages.Add(new AssistantChatMessage(completion));
                    await DisplayAssistantMessage(completion.Content[0].Text);

                    break;

                case ChatFinishReason.ToolCalls:
                    _messages.Add(new AssistantChatMessage(completion));

                    foreach (ChatToolCall toolCall in completion.ToolCalls)
                    {
                        await DisplayState($"Calling Tool : {toolCall.FunctionName}");

                        var result = await _toolBelt.CallTool(toolCall);
                        _messages.Add(new ToolChatMessage(toolCall.Id, result));
                    }

                    break;

                default:
                    throw new NotImplementedException($"{completion.FinishReason} handler not implemented");
            }

        } while (completion.FinishReason != ChatFinishReason.Stop);

    }

    private async Task DisplayAssistantMessage(string message, bool resumeListening = true)
    {
        await _speechRecognition.PauseListening();
        var oldConsoleColor = Console.ForegroundColor;
        Console.WriteLine();
        Console.Write($"[ASSISTANT]: ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ForegroundColor = oldConsoleColor;
        await DisplayState("Talking");
        await _voice.Say(message);
        if (resumeListening) {
            Console.Write("[USER]: ");
            await _speechRecognition.StartListening();
        }
    }

    private async Task DisplayState(string state)
    {
        try
        {
            await _connection.InvokeAsync("StateChanged", state);

        }
        catch (Exception)
        {

        }

        int x = Console.CursorLeft;
        int y = Console.CursorTop;

        var oldConsoleColor = Console.ForegroundColor;
        Console.SetCursorPosition(0,0);      
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write(state.PadRight(50-state.Length));
        Console.SetCursorPosition(x, y);
        
        Console.ForegroundColor = oldConsoleColor;
    }

}

