using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Cot.Generated;

public sealed class GetCotReportSeriesResponse
{
    [JsonPropertyName("fromDate")]
    public string? FromDate { get; set; }

    [JsonPropertyName("group")]
    public string? Group { get; set; }

    [JsonPropertyName("instrumentId")]
    public string? InstrumentId { get; set; }

    [JsonPropertyName("instrumentName")]
    public string? InstrumentName { get; set; }

    [JsonPropertyName("instrumentSlug")]
    public string? InstrumentSlug { get; set; }

    [JsonPropertyName("items")]
    public IReadOnlyList<GetCotReportSeriesItemResponse>? Items { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }

    [JsonPropertyName("toDate")]
    public string? ToDate { get; set; }
}
