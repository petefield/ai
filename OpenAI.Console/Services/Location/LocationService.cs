namespace OpenAI.Console.Services.Location;

internal class LocationService()
{
    public  Task<LocationInfo> GetLocationData() => Task.FromResult( new LocationInfo(
            Ip: string.Empty,
            IsEuropeanUnion: false,
            L10n: new Localization(
                CurrencyName: "Pound",
                CurrencyCode: "GBP", 
                CurrencySymbol:"£",
                LangCodes: ["en-GB"]),
            LocationData: new LocationData(
                CountryName: "United Kingdom",
                CountryCode: "UK",
                StateName: "Surrey",
                StateCode: string.Empty,
                CityName: "Dorking",
                CityGeonamesId: 0,
                Lat: 51.2327334909231,
                Lng: -0.3113196959545011,
                Tz: "GMT",
                ContinentCode: "Eur")));

}
