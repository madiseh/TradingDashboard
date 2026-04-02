using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Cot.Owned;

public sealed class CotReportSeriesResponse
{
    [JsonPropertyName("instrumentId")]
    public Guid InstrumentId { get; set; }

    [JsonPropertyName("instrumentSlug")]
    public string InstrumentSlug { get; set; } = string.Empty;

    [JsonPropertyName("instrumentName")]
    public string InstrumentName { get; set; } = string.Empty;

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("group")]
    public string Group { get; set; } = string.Empty;

    [JsonPropertyName("fromDate")]
    public DateOnly FromDate { get; set; }

    [JsonPropertyName("toDate")]
    public DateOnly ToDate { get; set; }

    [JsonPropertyName("items")]
    public List<CotReportSeriesItemResponse> Items { get; set; } = [];
}

public sealed class CotReportSeriesItemResponse
{
    [JsonPropertyName("reportDate")]
    public DateOnly ReportDate { get; set; }

    [JsonPropertyName("long")]
    public int? Long { get; set; }

    [JsonPropertyName("short")]
    public int? Short { get; set; }

    [JsonPropertyName("spread")]
    public int? Spread { get; set; }

    [JsonPropertyName("netPosition")]
    public int? NetPosition { get; set; }

    [JsonPropertyName("openInterest")]
    public int? OpenInterest { get; set; }
}
