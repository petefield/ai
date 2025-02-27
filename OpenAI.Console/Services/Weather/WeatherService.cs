using Microsoft.Extensions.Options;
using OpenAI.Console.Configuration;
using OpenWeatherAPI;
using System.Net.Http.Json;

namespace OpenAI.Console.Services.Weather;

internal class WeatherService
{
    private readonly OpenWeatherApiClient openWeatherAPI;

    public WeatherService(IOptions<WeatherServiceConfiguration> weatherServiceConfiguration )
    {
        ArgumentNullException.ThrowIfNull(weatherServiceConfiguration.Value.Key);

        openWeatherAPI = new OpenWeatherAPI.OpenWeatherApiClient(weatherServiceConfiguration.Value.Key);
    }

    public async Task<WeatherData> GetWeatherData(string latitude, string longditude)
    {
        var httpClient = new HttpClient();

        try
        {
            var r = await httpClient.GetFromJsonAsync<WeatherData>($"https://api.openweathermap.org/data/2.5/weather?lat={latitude}&lon={longditude}&appid=f68bf144fb5ef874ba4c6fefc0506f6c");
            return r;
        }
        catch (Exception ex)
        {

            throw;
        }


    }
}
