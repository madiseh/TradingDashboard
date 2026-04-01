using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Cot.Generated;

public sealed class GetCotLatestReportMetricResponse
{
    [JsonPropertyName("change")]
    public int? Change { get; set; }

    [JsonPropertyName("percentOfOpenInterest")]
    public double? PercentOfOpenInterest { get; set; }

    [JsonPropertyName("positions")]
    public int? Positions { get; set; }

    [JsonPropertyName("traders")]
    public int? Traders { get; set; }
}
