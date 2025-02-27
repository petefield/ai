using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Options;
using OpenAI.Console.Configuration;

namespace SpeechToText;

internal class Voice(IOptions<SpeechConfiguration> config)
{
    private readonly SpeechSynthesizer _speechSynthesizer =
        new(SpeechConfig.FromSubscription(config.Value.Key, config.Value.Region));

    public async Task Say(string text)
    {
        await _speechSynthesizer.SpeakTextAsync(text);
    }
}
