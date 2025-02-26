using System.Collections.Generic;
using System.Text.Json;

public record LocationInfo(
    string Ip,
    bool IsEuropeanUnion,
    Localization L10n,
    LocationData LocationData
)
{
    public override string ToString() => JsonSerializer.Serialize(this);
    
}

public record Localization(
    string CurrencyName,
    string CurrencyCode,
    string CurrencySymbol,
    List<string> LangCodes
);

public record LocationData(
    string CountryName,
    string CountryCode,
    string StateName,
    string StateCode,
    string CityName,
    int CityGeonamesId,
    double Lat,
    double Lng,
    string Tz,
    string ContinentCode
);