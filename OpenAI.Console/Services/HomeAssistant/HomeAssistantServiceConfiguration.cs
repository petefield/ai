namespace OpenAI.Console.Configuration;

internal record HomeAssistantServiceConfiguration()
{
    public string Endpoint { get; set; } =  string.Empty;
    public string? Key { get; set; }
}
