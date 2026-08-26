namespace WeatherDashboard.Models;

/// <summary>
/// WMO weather_code 를 이모지 + 한글 설명으로 표현한 값.
/// </summary>
public readonly record struct WeatherCondition(int Code, string Emoji, string Description)
{
    public string Display => $"{Emoji} {Description}";
}

/// <summary>
/// 지오코딩 결과(도시 좌표/표시명).
/// </summary>
public sealed class GeoLocation
{
    public required string Name { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Country { get; init; }
    public string? Admin1 { get; init; }
    public string? Timezone { get; init; }

    /// <summary>"서울, 대한민국" 형태의 표시명.</summary>
    public string DisplayName =>
        string.IsNullOrWhiteSpace(Admin1) && string.IsNullOrWhiteSpace(Country)
            ? Name
            : string.Join(", ", new[] { Name, Admin1, Country }
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct());
}

/// <summary>현재 날씨(UI용).</summary>
public sealed class CurrentWeather
{
    public DateTime Time { get; init; }
    public double Temperature { get; init; }
    public double ApparentTemperature { get; init; }
    public int RelativeHumidity { get; init; }
    public double WindSpeed { get; init; }
    public WeatherCondition Condition { get; init; }
}

/// <summary>시간별 예보 항목(UI용).</summary>
public sealed class HourlyForecastItem
{
    public DateTime Time { get; init; }
    public double Temperature { get; init; }
    public int? PrecipitationProbability { get; init; }
    public WeatherCondition Condition { get; init; }
}

/// <summary>일별 예보 항목(UI용).</summary>
public sealed class DailyForecastItem
{
    public DateOnly Date { get; init; }
    public double TemperatureMax { get; init; }
    public double TemperatureMin { get; init; }
    public int? PrecipitationProbabilityMax { get; init; }
    public WeatherCondition Condition { get; init; }
}

/// <summary>
/// 현재 + 시간별 + 일별 예보를 통합한 결과. 컴포넌트가 소비하는 최상위 모델.
/// </summary>
public sealed class WeatherResult
{
    public required GeoLocation Location { get; init; }
    public required CurrentWeather Current { get; init; }
    public IReadOnlyList<HourlyForecastItem> Hourly { get; init; } = Array.Empty<HourlyForecastItem>();
    public IReadOnlyList<DailyForecastItem> Daily { get; init; } = Array.Empty<DailyForecastItem>();
}
