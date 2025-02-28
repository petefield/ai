// See https://aka.ms/new-console-template for more information
using Microsoft.AspNetCore.SignalR.Client;

Console.ForegroundColor = ConsoleColor.Green;
Console.CursorVisible = false;
Console.WindowHeight = 50;

var frames = new Dictionary<string, string[]> {
    { "mouthClosed",  File.ReadAllLines(@"C:\Users\N19284\source\ai\ai\OpenAI.Face\face-closed.txt")},
    { "mouthClosedBlink",  File.ReadAllLines(@"C:\Users\N19284\source\ai\ai\OpenAI.Face\face-closed-blink.txt")},
    { "mouthOpen",  File.ReadAllLines(@"C:\Users\N19284\source\ai\ai\OpenAI.Face\face-open.txt")}
};


string state = "mouthClosed";

DrawFrame(state);

var connection = new HubConnectionBuilder()
    .WithUrl("http://localhost:5057/OpenAIAssitantHub")
    .Build();

connection.On<string>("StateChanged", (state) => {
    switch (state.ToLower())
    {
        case "talking":
            state = "mouthOpen";
            break;
        case "listening":
            state = "mouthClosed";
            break;
        case "working":
            state = "mouthClosed";
            break;
    }

    DrawFrame(state);
});

await OpenConnection(connection);


var random = new Random();

while (true) {
    await Task.Delay(random.Next(1500, 3500));

    DrawFrame($"{state}Blink");

    await Task.Delay(random.Next(500));
    DrawFrame(state);
}




async Task OpenConnection(HubConnection connection)
{

    var connectionAttemptsRemaining = 50;

    while (true)
    {
        try
        {
            await connection.StartAsync();
            break;
        }
        catch (Exception ex)
        {
            if (connectionAttemptsRemaining == 0)
            {
                Console.WriteLine("Failed to connect to the server. Please try again later.");
                throw;
            }
            await Task.Delay(500);
            connectionAttemptsRemaining--;
        }
    }
}




void DrawFrame(string frame)
{
    Console.SetCursorPosition(0, 0);

    if (frames.TryGetValue(frame, out var lines))
    {
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }
}

