using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Cot.Generated;

public sealed class GetCotReportSeriesItemResponse
{
    [JsonPropertyName("long")]
    public int? Long { get; set; }

    [JsonPropertyName("netPosition")]
    public int? NetPosition { get; set; }

    [JsonPropertyName("openInterest")]
    public int? OpenInterest { get; set; }

    [JsonPropertyName("reportDate")]
    public string? ReportDate { get; set; }

    [JsonPropertyName("short")]
    public int? Short { get; set; }

    [JsonPropertyName("spread")]
    public int? Spread { get; set; }
}
