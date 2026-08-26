using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using WeatherDashboard.Models;

namespace WeatherDashboard.Services;

/// <summary>
/// Open-Meteo(키 불필요) API를 호출하는 <see cref="IWeatherService"/> 구현.
/// 지오코딩 + 예보(current/hourly/daily)를 통합한다.
/// </summary>
public sealed class WeatherService : IWeatherService
{
    private const string GeocodingUrl = "https://geocoding-api.open-meteo.com/v1/search";
    private const string ForecastUrl = "https://api.open-meteo.com/v1/forecast";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<GeoLocation?> SearchCityAsync(string city, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return null;
        }

        var url = $"{GeocodingUrl}?name={Uri.EscapeDataString(city.Trim())}&count=1&language=ko&format=json";
        var dto = await _httpClient.GetFromJsonAsync<GeocodingResponseDto>(url, JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        var result = dto?.Results?.FirstOrDefault();
        if (result is null)
        {
            return null;
        }

        return new GeoLocation
        {
            Name = result.Name,
            Latitude = result.Latitude,
            Longitude = result.Longitude,
            Country = result.Country,
            Admin1 = result.Admin1,
            Timezone = result.Timezone,
        };
    }

    public async Task<WeatherResult> GetWeatherAsync(GeoLocation location, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(location);

        var lat = location.Latitude.ToString(CultureInfo.InvariantCulture);
        var lon = location.Longitude.ToString(CultureInfo.InvariantCulture);

        var url =
            $"{ForecastUrl}?latitude={lat}&longitude={lon}" +
            "&current=temperature_2m,apparent_temperature,relative_humidity_2m,wind_speed_10m,weather_code" +
            "&hourly=temperature_2m,weather_code,precipitation_probability" +
            "&daily=weather_code,temperature_2m_max,temperature_2m_min,precipitation_probability_max" +
            "&timezone=auto&wind_speed_unit=ms&forecast_days=6";

        var dto = await _httpClient.GetFromJsonAsync<ForecastResponseDto>(url, JsonOptions, cancellationToken)
            .ConfigureAwait(false);

        if (dto is null)
        {
            throw new InvalidOperationException("Open-Meteo 예보 응답이 비어 있습니다.");
        }

        return MapToResult(location, dto);
    }

    public async Task<WeatherResult?> GetWeatherByCityAsync(string city, CancellationToken cancellationToken = default)
    {
        var location = await SearchCityAsync(city, cancellationToken).ConfigureAwait(false);
        if (location is null)
        {
            return null;
        }

        return await GetWeatherAsync(location, cancellationToken).ConfigureAwait(false);
    }

    public static WeatherResult MapToResult(GeoLocation location, ForecastResponseDto dto)
    {
        var current = MapCurrent(dto.Current);
        return new WeatherResult
        {
            Location = location,
            Current = current,
            Hourly = MapHourly(dto.Hourly),
            Daily = MapDaily(dto.Daily),
        };
    }

    private static CurrentWeather MapCurrent(CurrentDto? current)
    {
        if (current is null)
        {
            return new CurrentWeather { Condition = WeatherCodeMapper.Describe(0) };
        }

        return new CurrentWeather
        {
            Time = ParseDateTime(current.Time),
            Temperature = current.Temperature,
            ApparentTemperature = current.ApparentTemperature,
            RelativeHumidity = current.RelativeHumidity,
            WindSpeed = current.WindSpeed,
            Condition = WeatherCodeMapper.Describe(current.WeatherCode),
        };
    }

    private static IReadOnlyList<HourlyForecastItem> MapHourly(HourlyDto? hourly)
    {
        if (hourly is null)
        {
            return Array.Empty<HourlyForecastItem>();
        }

        var count = hourly.Time.Count;
        var items = new List<HourlyForecastItem>(count);
        for (var i = 0; i < count; i++)
        {
            items.Add(new HourlyForecastItem
            {
                Time = ParseDateTime(hourly.Time[i]),
                Temperature = ValueAt(hourly.Temperature, i),
                PrecipitationProbability = ValueAt(hourly.PrecipitationProbability, i),
                Condition = WeatherCodeMapper.Describe((int)ValueAt(hourly.WeatherCode, i)),
            });
        }

        return items;
    }

    private static IReadOnlyList<DailyForecastItem> MapDaily(DailyDto? daily)
    {
        if (daily is null)
        {
            return Array.Empty<DailyForecastItem>();
        }

        var count = daily.Time.Count;
        var items = new List<DailyForecastItem>(count);
        for (var i = 0; i < count; i++)
        {
            items.Add(new DailyForecastItem
            {
                Date = ParseDate(daily.Time[i]),
                TemperatureMax = ValueAt(daily.TemperatureMax, i),
                TemperatureMin = ValueAt(daily.TemperatureMin, i),
                PrecipitationProbabilityMax = ValueAt(daily.PrecipitationProbabilityMax, i),
                Condition = WeatherCodeMapper.Describe((int)ValueAt(daily.WeatherCode, i)),
            });
        }

        return items;
    }

    private static T ValueAt<T>(IReadOnlyList<T> list, int index) where T : struct =>
        index < list.Count ? list[index] : default;

    private static T? ValueAt<T>(IReadOnlyList<T?> list, int index) where T : struct =>
        index < list.Count ? list[index] : null;

    private static DateTime ParseDateTime(string? value) =>
        DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt)
            ? dt
            : default;

    private static DateOnly ParseDate(string? value) =>
        DateOnly.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var d)
            ? d
            : default;
}
