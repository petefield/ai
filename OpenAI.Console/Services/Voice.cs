using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Options;
using OpenAI.Console.Configuration;

namespace SpeechToText;

internal class Voice
{
    private readonly SpeechSynthesizer _speechSynthesizer;


    public Voice(IOptions<SpeechConfiguration> config)
    {
        var speechConfig = SpeechConfig.FromSubscription(config.Value.Key, config.Value.Region);

        speechConfig.SpeechSynthesisVoiceName = "Pete Field_20250227_8588";

        _speechSynthesizer = new SpeechSynthesizer(speechConfig);

    }




    public async Task Say(string text)
    {
        await _speechSynthesizer.SpeakTextAsync(text);
    }
}
