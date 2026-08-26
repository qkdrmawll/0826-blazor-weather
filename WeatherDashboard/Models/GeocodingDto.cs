using System.Text.Json.Serialization;

namespace WeatherDashboard.Models;

/// <summary>
/// Open-Meteo 지오코딩 API(search) 응답 DTO.
/// </summary>
public sealed class GeocodingResponseDto
{
    [JsonPropertyName("results")]
    public List<GeocodingResultDto>? Results { get; set; }
}

public sealed class GeocodingResultDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("country")]
    public string? Country { get; set; }

    [JsonPropertyName("admin1")]
    public string? Admin1 { get; set; }

    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }
}
