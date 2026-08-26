using System.Text.Json;
using WeatherDashboard.Models;
using WeatherDashboard.Services;

namespace WeatherDashboard.Tests;

public class ForecastDeserializationTests
{
    private static readonly JsonSerializerOptions Options = new(JsonSerializerDefaults.Web);

    private const string GeocodingJson = """
    {
      "results": [
        {
          "id": 1835848,
          "name": "서울",
          "latitude": 37.566,
          "longitude": 126.9784,
          "country": "대한민국",
          "admin1": "서울특별시",
          "timezone": "Asia/Seoul"
        }
      ]
    }
    """;

    private const string ForecastJson = """
    {
      "latitude": 37.5,
      "longitude": 127.0,
      "timezone": "Asia/Seoul",
      "current": {
        "time": "2026-08-26T16:00",
        "temperature_2m": 27.3,
        "apparent_temperature": 29.1,
        "relative_humidity_2m": 65,
        "wind_speed_10m": 2.4,
        "weather_code": 61
      },
      "hourly": {
        "time": ["2026-08-26T16:00", "2026-08-26T17:00"],
        "temperature_2m": [27.3, 26.8],
        "weather_code": [61, 3],
        "precipitation_probability": [80, null]
      },
      "daily": {
        "time": ["2026-08-26", "2026-08-27"],
        "weather_code": [61, 0],
        "temperature_2m_max": [29.0, 30.5],
        "temperature_2m_min": [22.1, 21.7],
        "precipitation_probability_max": [90, 10]
      }
    }
    """;

    [Fact]
    public void Deserialize_Geocoding_MapsFields()
    {
        var dto = JsonSerializer.Deserialize<GeocodingResponseDto>(GeocodingJson, Options);

        var result = Assert.Single(dto!.Results!);
        Assert.Equal("서울", result.Name);
        Assert.Equal(37.566, result.Latitude, 3);
        Assert.Equal("대한민국", result.Country);
        Assert.Equal("Asia/Seoul", result.Timezone);
    }

    [Fact]
    public void Deserialize_Forecast_MapsCurrentHourlyDaily()
    {
        var dto = JsonSerializer.Deserialize<ForecastResponseDto>(ForecastJson, Options);

        Assert.NotNull(dto);
        Assert.Equal(61, dto!.Current!.WeatherCode);
        Assert.Equal(65, dto.Current.RelativeHumidity);
        Assert.Equal(2, dto.Hourly!.Time.Count);
        Assert.Null(dto.Hourly.PrecipitationProbability[1]);
        Assert.Equal(2, dto.Daily!.Time.Count);
        Assert.Equal(90, dto.Daily.PrecipitationProbabilityMax[0]);
    }

    [Fact]
    public void MapToResult_BuildsIntegratedResult()
    {
        var dto = JsonSerializer.Deserialize<ForecastResponseDto>(ForecastJson, Options)!;
        var location = new GeoLocation
        {
            Name = "서울",
            Latitude = 37.5,
            Longitude = 127.0,
            Country = "대한민국",
        };

        var result = WeatherService.MapToResult(location, dto);

        Assert.Equal("서울", result.Location.Name);
        Assert.Equal(27.3, result.Current.Temperature, 1);
        Assert.Equal("🌧️ 약한 비", result.Current.Condition.Display);
        Assert.Equal(2, result.Hourly.Count);
        Assert.Equal(80, result.Hourly[0].PrecipitationProbability);
        Assert.Null(result.Hourly[1].PrecipitationProbability);
        Assert.Equal(2, result.Daily.Count);
        Assert.Equal(new DateOnly(2026, 8, 26), result.Daily[0].Date);
        Assert.Equal("☀️ 맑음", result.Daily[1].Condition.Display);
    }
}
