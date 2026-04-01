using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Cot.Generated;

public sealed class GetCotLatestReportCategoryResponse
{
    [JsonPropertyName("key")]
    public string? Key { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("long")]
    public object? Long { get; set; }

    [JsonPropertyName("short")]
    public object? Short { get; set; }

    [JsonPropertyName("spread")]
    public object? Spread { get; set; }
}
