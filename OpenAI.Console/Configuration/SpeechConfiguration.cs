namespace OpenAI.Console.Configuration;

internal record SpeechConfiguration()
{
    public string? Region { get; set; }
    public string? Key { get; set; }
}
