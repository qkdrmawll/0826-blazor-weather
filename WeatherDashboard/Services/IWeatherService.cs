using WeatherDashboard.Models;

namespace WeatherDashboard.Services;

/// <summary>
/// Open-Meteo 기반 날씨 조회 서비스.
/// </summary>
public interface IWeatherService
{
    /// <summary>도시명(한/영)으로 좌표/표시명을 검색한다. 결과가 없으면 null.</summary>
    Task<GeoLocation?> SearchCityAsync(string city, CancellationToken cancellationToken = default);

    /// <summary>좌표로 현재/시간별/일별 예보를 통합 조회한다.</summary>
    Task<WeatherResult> GetWeatherAsync(GeoLocation location, CancellationToken cancellationToken = default);

    /// <summary>도시명으로 검색 후 예보까지 한 번에 조회한다. 도시를 찾지 못하면 null.</summary>
    Task<WeatherResult?> GetWeatherByCityAsync(string city, CancellationToken cancellationToken = default);
}
