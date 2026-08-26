using System.Text.Json.Serialization;

namespace WeatherDashboard.Models;

/// <summary>
/// Open-Meteo 날씨 예보 API(forecast) 응답 DTO.
/// </summary>
public sealed class ForecastResponseDto
{
    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }

    [JsonPropertyName("current")]
    public CurrentDto? Current { get; set; }

    [JsonPropertyName("hourly")]
    public HourlyDto? Hourly { get; set; }

    [JsonPropertyName("daily")]
    public DailyDto? Daily { get; set; }
}

public sealed class CurrentDto
{
    [JsonPropertyName("time")]
    public string? Time { get; set; }

    [JsonPropertyName("temperature_2m")]
    public double Temperature { get; set; }

    [JsonPropertyName("apparent_temperature")]
    public double ApparentTemperature { get; set; }

    [JsonPropertyName("relative_humidity_2m")]
    public int RelativeHumidity { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double WindSpeed { get; set; }

    [JsonPropertyName("weather_code")]
    public int WeatherCode { get; set; }
}

public sealed class HourlyDto
{
    [JsonPropertyName("time")]
    public List<string> Time { get; set; } = new();

    [JsonPropertyName("temperature_2m")]
    public List<double> Temperature { get; set; } = new();

    [JsonPropertyName("weather_code")]
    public List<int> WeatherCode { get; set; } = new();

    [JsonPropertyName("precipitation_probability")]
    public List<int?> PrecipitationProbability { get; set; } = new();
}

public sealed class DailyDto
{
    [JsonPropertyName("time")]
    public List<string> Time { get; set; } = new();

    [JsonPropertyName("weather_code")]
    public List<int> WeatherCode { get; set; } = new();

    [JsonPropertyName("temperature_2m_max")]
    public List<double> TemperatureMax { get; set; } = new();

    [JsonPropertyName("temperature_2m_min")]
    public List<double> TemperatureMin { get; set; } = new();

    [JsonPropertyName("precipitation_probability_max")]
    public List<int?> PrecipitationProbabilityMax { get; set; } = new();
}
