using WeatherDashboard.Services;

namespace WeatherDashboard.Tests;

public class WeatherCodeMapperTests
{
    [Theory]
    [InlineData(0, "☀️", "맑음")]
    [InlineData(3, "☁️", "흐림")]
    [InlineData(61, "🌧️", "약한 비")]
    [InlineData(95, "⛈️", "뇌우")]
    [InlineData(71, "🌨️", "약한 눈")]
    public void Describe_KnownCode_ReturnsExpectedEmojiAndDescription(int code, string emoji, string description)
    {
        var condition = WeatherCodeMapper.Describe(code);

        Assert.Equal(code, condition.Code);
        Assert.Equal(emoji, condition.Emoji);
        Assert.Equal(description, condition.Description);
        Assert.Equal($"{emoji} {description}", condition.Display);
    }

    [Fact]
    public void Describe_UnknownCode_ReturnsUnknownFallback()
    {
        var condition = WeatherCodeMapper.Describe(12345);

        Assert.Equal("❓", condition.Emoji);
        Assert.Equal("알 수 없음", condition.Description);
    }

    [Fact]
    public void GetEmoji_And_GetDescription_ReturnParts()
    {
        Assert.Equal("☀️", WeatherCodeMapper.GetEmoji(0));
        Assert.Equal("맑음", WeatherCodeMapper.GetDescription(0));
    }
}
