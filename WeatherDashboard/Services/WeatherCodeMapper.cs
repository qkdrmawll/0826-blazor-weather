using WeatherDashboard.Models;

namespace WeatherDashboard.Services;

/// <summary>
/// WMO weather interpretation code(WW) → 이모지 + 한글 설명 매핑 유틸.
/// 참고: https://open-meteo.com/en/docs (WMO Weather interpretation codes)
/// </summary>
public static class WeatherCodeMapper
{
    private static readonly IReadOnlyDictionary<int, (string Emoji, string Description)> Map =
        new Dictionary<int, (string, string)>
        {
            [0] = ("☀️", "맑음"),
            [1] = ("🌤️", "대체로 맑음"),
            [2] = ("⛅", "부분적으로 흐림"),
            [3] = ("☁️", "흐림"),
            [45] = ("🌫️", "안개"),
            [48] = ("🌫️", "상고대 안개"),
            [51] = ("🌦️", "약한 이슬비"),
            [53] = ("🌦️", "이슬비"),
            [55] = ("🌦️", "강한 이슬비"),
            [56] = ("🌧️", "약한 어는 이슬비"),
            [57] = ("🌧️", "강한 어는 이슬비"),
            [61] = ("🌧️", "약한 비"),
            [63] = ("🌧️", "비"),
            [65] = ("🌧️", "강한 비"),
            [66] = ("🌧️", "약한 어는 비"),
            [67] = ("🌧️", "강한 어는 비"),
            [71] = ("🌨️", "약한 눈"),
            [73] = ("🌨️", "눈"),
            [75] = ("❄️", "강한 눈"),
            [77] = ("🌨️", "싸락눈"),
            [80] = ("🌦️", "약한 소나기"),
            [81] = ("🌧️", "소나기"),
            [82] = ("⛈️", "강한 소나기"),
            [85] = ("🌨️", "약한 소낙눈"),
            [86] = ("❄️", "강한 소낙눈"),
            [95] = ("⛈️", "뇌우"),
            [96] = ("⛈️", "약한 우박을 동반한 뇌우"),
            [99] = ("⛈️", "강한 우박을 동반한 뇌우"),
        };

    /// <summary>알 수 없는 코드에 사용할 기본값.</summary>
    private static readonly (string Emoji, string Description) Unknown = ("❓", "알 수 없음");

    /// <summary>WMO 코드를 <see cref="WeatherCondition"/> 로 변환한다.</summary>
    public static WeatherCondition Describe(int code)
    {
        var (emoji, description) = Map.TryGetValue(code, out var value) ? value : Unknown;
        return new WeatherCondition(code, emoji, description);
    }

    /// <summary>이모지만 반환.</summary>
    public static string GetEmoji(int code) => Describe(code).Emoji;

    /// <summary>한글 설명만 반환.</summary>
    public static string GetDescription(int code) => Describe(code).Description;
}
