using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Options;
using OpenAI.Console.Configuration;

namespace OpenAI.Console.Services;

internal class SpeechRecognition
{
    private readonly SpeechRecognizer _speechRecognizer;
    private readonly KeywordRecognizer _keywordRecognizer;
    private readonly KeywordRecognitionModel _keywordModel;

    public Action<SpeechRecognition, string>? OnSpeechRecognised { get; set; }
    public Action<SpeechRecognition, string>? OnKeyWordRecognised { get; set; }
    public Action<SpeechRecognition, string>? OnStateChanged { get; set; }

    public SpeechRecognition(IOptions<SpeechConfiguration> config)
    {

        _keywordModel = KeywordRecognitionModel.FromFile(@"C:\Users\N19284\source\ai\ai\OpenAI.Console\kws.table");

        var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        var speechConfig = SpeechConfig.FromSubscription(config.Value.Key, config.Value.Region);

        _keywordRecognizer = new KeywordRecognizer(audioConfig);

        _speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

        _speechRecognizer.Recognized += (s, e) =>
        {
            if (e.Result.Reason == ResultReason.RecognizedSpeech)
            {
                if (!string.IsNullOrWhiteSpace(e.Result.Text))
                {
                    OnSpeechRecognised?.Invoke(this, e.Result.Text);
                }
            }
            else if (e.Result.Reason == ResultReason.NoMatch)
            {
                System.Console.WriteLine($"NOMATCH: Speech could not be recognized.");
            }
        };

        _speechRecognizer.Canceled += (s, e) =>
        {
            System.Console.WriteLine($"CANCELED: Reason={e.Reason}");

            if (e.Reason == CancellationReason.Error)
            {
                System.Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                System.Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                System.Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
            }
        };
    }

    public Task StartListeningForKeyWord()
    {
        var t = Task.Run(async () => {
            KeywordRecognitionResult result;
            do
            {
                OnStateChanged?.Invoke(this, "Listening For Keyword");

                result = await _keywordRecognizer.RecognizeOnceAsync(_keywordModel);

            } while (result.Reason != ResultReason.RecognizedKeyword);

            OnKeyWordRecognised?.Invoke(this, result.Text);

        });

        return t;
    }

    public async Task PauseListening()
    {
        await _speechRecognizer.StopContinuousRecognitionAsync();
        OnStateChanged?.Invoke(this, "Paused");
    }

    public async Task StartListening() {
        await _speechRecognizer.StartContinuousRecognitionAsync();
        OnStateChanged?.Invoke(this, "Listening");
    }
}
