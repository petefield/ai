using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Options;
using OpenAI.Console.Configuration;

namespace SpeechToText
{
    internal class SpeechRecognition
    {
        private readonly SpeechRecognizer _speechRecognizer;

        private readonly TaskCompletionSource _stop = new();

        public SpeechRecognition(IOptions<SpeechConfiguration> config)
        {
            _speechRecognizer = new SpeechRecognizer(
                 SpeechConfig.FromSubscription(config.Value.Key, config.Value.Region),
                 AudioConfig.FromDefaultMicrophoneInput());

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
                    Console.WriteLine($"NOMATCH: Speech could not be recognized.");
                }
            };

            _speechRecognizer.Canceled += (s, e) =>
            {
                Console.WriteLine($"CANCELED: Reason={e.Reason}");

                if (e.Reason == CancellationReason.Error)
                {
                    Console.WriteLine($"CANCELED: ErrorCode={e.ErrorCode}");
                    Console.WriteLine($"CANCELED: ErrorDetails={e.ErrorDetails}");
                    Console.WriteLine($"CANCELED: Did you set the speech resource key and region values?");
                }
            };
        }

        public Action<SpeechRecognition, string>? OnSpeechRecognised { get; set; }

        public async Task StopListening()
        {
            await _speechRecognizer.StopContinuousRecognitionAsync();
        }

        public async Task StartListening()
        {
            await _speechRecognizer.StartContinuousRecognitionAsync();
        }
    }
}
