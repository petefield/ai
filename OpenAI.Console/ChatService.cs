using OpenAI.Chat;
using System.ClientModel;
using OpenAI.Console.Tools;
using Microsoft.Extensions.Options;
using OpenAI.Console;
using SpeechToText;

internal class ChatService
{
    private readonly ToolBelt _toolBelt;
    private readonly Voice _voice;
    private readonly SpeechRecognition _speechRecognition;
    private readonly List<ChatMessage> _messages = [];
    private readonly ChatClient? _client = null;
    private readonly ChatCompletionOptions _chatCompletionOptions = new();

    public ChatService(ToolBelt toolBelt, 
        Voice voice, 
        SpeechRecognition speechRecognition, 
        IOptions<OpenAIConfiguration> openAIConfiguration)
    {
        _toolBelt = toolBelt;
        _voice = voice;
        _speechRecognition = speechRecognition;

        _client = new(
            model: openAIConfiguration.Value.Model,
            credential: new ApiKeyCredential(openAIConfiguration.Value.Key)
        );

        _messages.Add(new SystemChatMessage("""
            You are a very rude, patroizing assistant. 
            You speak like a pirate.
            Keep your answers as short, snappy and rude as possible.
         """));

        _chatCompletionOptions.Tools.AddRange(toolBelt.GetTools());

        _speechRecognition.OnSpeechRecognised += async (s, e) =>
        {
            Console.Write(e);
            await Chat(e);
        };        
    }

    public async Task Start() {

        Console.Write("[USER]: ");

        await _speechRecognition.StartListening();

        while (true)
        {
            await Chat(Console.ReadLine());
        }
    }
    private async Task Chat(string? userInput)
    {
        if (string.IsNullOrEmpty(userInput))
            return;

        _messages.Add(new UserChatMessage(userInput));

        ChatCompletion completion;
        do
        {
            completion = _client!.CompleteChat(_messages, _chatCompletionOptions);

            switch (completion.FinishReason)
            {
                case ChatFinishReason.Stop:
                    _messages.Add(new AssistantChatMessage(completion));
                    var responseText = completion.Content[0].Text;

                    Console.WriteLine();
                    Console.Write($"[ASSISTANT]: ");

                    var oldConsoleColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{responseText}");
                    Console.ForegroundColor = oldConsoleColor;

                    await _speechRecognition.StopListening();
                    await _voice.Say(responseText);
                    await _speechRecognition.StartListening();

                    Console.Write("[USER]: ");
                    break;

                case ChatFinishReason.ToolCalls:
                    _messages.Add(new AssistantChatMessage(completion));

                    foreach (ChatToolCall toolCall in completion.ToolCalls)
                    {
                        var result = await _toolBelt.CallTool(toolCall);
                        _messages.Add(new ToolChatMessage(toolCall.Id, result));
                    }

                    break;

                default:
                    throw new NotImplementedException($"{completion.FinishReason} handler not implemented");
            }

        } while (completion.FinishReason != ChatFinishReason.Stop);

    }

}

