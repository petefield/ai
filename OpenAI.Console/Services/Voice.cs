using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Options;
using OpenAI.Console.Configuration;
using System.Xml.Linq;
using System;

namespace SpeechToText;

internal class Voice
{
    private readonly SpeechSynthesizer _speechSynthesizer;
    private string voiceName;

    public Voice(IOptions<SpeechConfiguration> config)
    {
        var speechConfig = SpeechConfig.FromSubscription(config.Value.Key, config.Value.Region);

        voiceName = config.Value.Voice ?? "en-US-AvaMultilingualNeural";

        _speechSynthesizer = new SpeechSynthesizer(speechConfig);

        _speechSynthesizer.SynthesisCompleted += (s, e) =>
        {
            Console.WriteLine(e.Result);
        };

        _speechSynthesizer.SynthesisCanceled += (s, e) =>
        {
            Console.WriteLine(e.Result);
        };

    }

    public async Task Say(string text)
    {
        var ssml = $"""
        <speak version='1.0' xmlns='https://www.w3.org/2001/10/synthesis' xmlns:mstts='http://www.w3.org/2001/mstts' xml:lang='en-US'>
            <voice name = '{voiceName}'>
                <prosody rate='+65.00%' pitch='+10Hz'>
                {text}
                </prosody>
            </voice>
        </speak>
        """;
        await _speechSynthesizer.SpeakSsmlAsync(ssml);
    }
}
