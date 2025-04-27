using OpenAI.Chat;
using System.ClientModel;
using OpenAI.Console.Tools;
using Microsoft.Extensions.Options;
using OpenAI.Console;
using SpeechToText;
using OpenAI.Console.Services;
using Microsoft.AspNetCore.SignalR.Client;
using OpenAI;
using System.Net;
using Microsoft.CognitiveServices.Speech;
using SignalRChat.Hubs;

internal class ChatService : IStart
{
    private readonly ToolBelt _toolBelt;
    private readonly Voice _voice;
    private readonly List<ChatMessage> _messages;
    private readonly ChatClient? _client = null;
    private readonly ChatCompletionOptions _chatCompletionOptions = new();
    private readonly HubConnection _connection;

    public ChatService(ToolBelt toolBelt,
        Voice voice,
        IOptions<OpenAIConfiguration> openAIConfiguration)
    {
        ArgumentNullException.ThrowIfNull(openAIConfiguration.Value.Key);

        _toolBelt = toolBelt;
        _voice = voice;

        var options = new OpenAIClientOptions();

        if (openAIConfiguration.Value.Endpoint is not null)
        {
            options.Endpoint = new(openAIConfiguration.Value.Endpoint);
        }

        _client = new(
            model: openAIConfiguration.Value.Model,
            credential: new ApiKeyCredential(openAIConfiguration.Value.Key),
            options
        );

        var sp = File.ReadAllText("SystemPrompt.txt");

        _messages = [new SystemChatMessage(sp)];

        _chatCompletionOptions.Tools.AddRange(toolBelt.GetTools());

        _connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5057/OpenAIAssitantHub")
            .Build();
    }

    private async Task ConnectToHub()
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
    }

    public async Task Start()
    {
        await ConnectToHub();

        _connection.On<string>(OpenAIAssitantHub.UserInputReceivedEvent, SendChatMessage);

        Console.WriteLine("Chat Service Started");
        await DisplayAssistantResponse("Hi.");

        Console.ReadLine();
    }
    
    private async Task SendChatMessage(string? userInput)
    {
        Console.Write($"[User]: {userInput}");

        if (string.IsNullOrEmpty(userInput))
            return;

        _messages.Add(new UserChatMessage(userInput));

        ChatCompletion completion;

        await SetState(JeevesState.Thinking);

        do
        {
            completion = _client!.CompleteChat(_messages, _chatCompletionOptions);

            switch (completion.FinishReason)
            {
                case ChatFinishReason.Stop:
                    _messages.Add(new AssistantChatMessage(completion));
                    await DisplayAssistantResponse(completion.Content[0].Text);
                    break;

                case ChatFinishReason.ToolCalls:
                    _messages.Add(new AssistantChatMessage(completion));

                    foreach (ChatToolCall toolCall in completion.ToolCalls)
                    {
                        await SetState(JeevesState.Processing);
                        var result = await _toolBelt.CallTool(toolCall);
                        _messages.Add(new ToolChatMessage(toolCall.Id, result));
                    }

                    break;

                default:
                    throw new NotImplementedException($"{completion.FinishReason} handler not implemented");
            }

        } while (completion.FinishReason != ChatFinishReason.Stop);
    }

    private async Task DisplayAssistantResponse(string message)
    {
        var t = Say(message);
        var oldConsoleColor = Console.ForegroundColor;
        Console.WriteLine();
        Console.Write($"[JEEVES]: ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ForegroundColor = oldConsoleColor;
        await t;
    }

    public async Task Say(string message)
    { 
        await SetState(JeevesState.Speaking);
        await _voice.Say(message);
        await SetState(JeevesState.Listening);
    }

    private async Task SetState(JeevesState state)
    {
        try
        {
            await _connection.InvokeAsync("StateChanged", state);
        }
        catch (Exception)
        {

        }

        //int x = Console.CursorLeft;
        //int y = Console.CursorTop;

        //var oldConsoleColor = Console.ForegroundColor;
        //Console.SetCursorPosition(0,0);      
        //Console.ForegroundColor = ConsoleColor.Green;
        //Console.Write(state.ToString().PadRight(50-state.ToString().Length));
        //Console.SetCursorPosition(x, y);
        //Console.ForegroundColor = oldConsoleColor;
    }
}

