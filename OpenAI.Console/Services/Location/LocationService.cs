using Newtonsoft.Json;
using System.Net.Http.Json;

namespace OpenAI.Console.Services.Location;

internal class LocationService(HttpClient HttpClient)
{
    public async Task<LocationInfo> GetLocationData()
    {
        HttpClient.DefaultRequestHeaders.Add("Fastah-Key", "81c8515f0010417f808c5b562ff27638");



        var response = await HttpClient.GetAsync("https://ep.api.getfastah.com/whereis/v1/json/78.105.243.58");

        response.EnsureSuccessStatusCode();

        var c = await response.Content.ReadAsStringAsync();

        var result =  System.Text.Json.JsonSerializer.Deserialize<LocationInfo>(c, new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive = true});


        if (result == null) throw new Exception();

        return result;
    }
}
