using OpenAI.Chat;
using System.Speech.Synthesis;
using System.Speech.Recognition;
using System.ClientModel;
using OpenAI.Console.Tools;

internal class ChatService(ToolBelt ToolBelt)
{
    List<ChatMessage> messages = [];
    ChatClient? client = null;

    ChatCompletionOptions options = new();

    public async Task Start() {

        client = new(
            model: "gpt-4o",
            credential: new ApiKeyCredential("")
        );

        var spsynthesizer = new SpeechSynthesizer() { 
            Rate = 1,
        };

        spsynthesizer.SelectVoice("Microsoft David Desktop");

        messages.Add(new SystemChatMessage("You are a polite and efficient assistant.  Keep your answers as short as possible wihout being rude."));

        foreach (var t in ToolBelt.GetTools())
        {
            options.Tools.Add(t);
        }

        //using SpeechRecognitionEngine recognizer = new SpeechRecognitionEngine(new System.Globalization.CultureInfo("en-GB"));
        //recognizer.LoadGrammar(new DictationGrammar());
        //recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(recognizer_SpeechRecognized);
        //recognizer.SetInputToDefaultAudioDevice();
        //recognizer.RecognizeAsync(RecognizeMode.Multiple);

        Console.Write("[USER]: ");

        while (true)
        {
            await Chat(Console.ReadLine());
        }

        async void recognizer_SpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            try
            {
                var input = e.Result.Text;

                if (string.IsNullOrEmpty(input))
                    return;

                Console.WriteLine(input);

                await Chat(e.Result.Text);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    private async Task Chat(string? userInput)
    {
        if (string.IsNullOrEmpty(userInput))
            return;

        messages.Add(new UserChatMessage(userInput));

        ChatCompletion completion;
        do
        {

            completion = client!.CompleteChat(messages, options);

            switch (completion.FinishReason)
            {
                case ChatFinishReason.Stop:
                    messages.Add(new AssistantChatMessage(completion));
                    Console.WriteLine();
                    Console.WriteLine($"[ASSISTANT]: {completion.Content[0].Text}");
                    Console.WriteLine();
                    Console.Write("[USER]: ");
                    break;

                case ChatFinishReason.ToolCalls:
                    messages.Add(new AssistantChatMessage(completion));

                    foreach (ChatToolCall toolCall in completion.ToolCalls)
                    {
                        var result = await ToolBelt.CallTool(toolCall);
                        messages.Add(new ToolChatMessage(toolCall.Id, result));
                        break;
                    }
                    break;

                default:
                    throw new NotImplementedException($"{completion.FinishReason} handler not implemented");

            }

        } while (completion.FinishReason != ChatFinishReason.Stop);

    }

}

