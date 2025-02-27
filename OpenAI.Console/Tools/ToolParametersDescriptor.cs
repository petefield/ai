namespace OpenAI.Console.Tools;


public class ToolParametersDescriptor()
{
    public string type { get; set; } = "object";
    public Dictionary<string, ToolParameterDescriptor> properties { get; set; } = [];
    public string[] required { get; set; } = [];
}

