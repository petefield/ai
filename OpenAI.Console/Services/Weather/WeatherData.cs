using System;
using System.Collections.Generic;

public record WeatherData(
    double Lat,
    double Lon,
    string Timezone,
    int TimezoneOffset,
    CurrentWeather Current,
    List<MinutelyForecast> Minutely,
    List<HourlyForecast> Hourly,
    List<DailyForecast> Daily,
    List<Alert> Alerts
)
{
    public override string ToString() => System.Text.Json.JsonSerializer.Serialize(this);
}

public record CurrentWeather(
    long Dt,
    long Sunrise,
    long Sunset,
    double Temp,
    double FeelsLike,
    int Pressure,
    int Humidity,
    double DewPoint,
    double Uvi,
    int Clouds,
    int Visibility,
    double WindSpeed,
    int WindDeg,
    double WindGust,
    List<WeatherCondition> Weather
);

public record MinutelyForecast(
    long Dt,
    double Precipitation
);

public record HourlyForecast(
    long Dt,
    double Temp,
    double FeelsLike,
    int Pressure,
    int Humidity,
    double DewPoint,
    double Uvi,
    int Clouds,
    int Visibility,
    double WindSpeed,
    int WindDeg,
    double WindGust,
    List<WeatherCondition> Weather,
    double Pop
);

public record DailyForecast(
    long Dt,
    long Sunrise,
    long Sunset,
    long Moonrise,
    long Moonset,
    double MoonPhase,
    string Summary,
    Temperature Temp,
    FeelsLike FeelsLike,
    int Pressure,
    int Humidity,
    double DewPoint,
    double WindSpeed,
    int WindDeg,
    double WindGust,
    List<WeatherCondition> Weather,
    int Clouds,
    double Pop,
    double? Rain,
    double Uvi
);

public record Temperature(
    double Day,
    double Min,
    double Max,
    double Night,
    double Eve,
    double Morn
);

public record FeelsLike(
    double Day,
    double Night,
    double Eve,
    double Morn
);

public record WeatherCondition(
    int Id,
    string Main,
    string Description,
    string Icon
);

public record Alert(
    string SenderName,
    string Event,
    long Start,
    long End,
    string Description,
    List<string> Tags
);