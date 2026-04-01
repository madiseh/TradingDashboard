using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Cot.Owned;

public sealed class CotLatestReportResponse
{
    [JsonPropertyName("instrumentId")]
    public Guid InstrumentId { get; set; }

    [JsonPropertyName("instrumentCode")]
    public string InstrumentCode { get; set; } = string.Empty;

    [JsonPropertyName("instrumentSlug")]
    public string InstrumentSlug { get; set; } = string.Empty;

    [JsonPropertyName("instrumentName")]
    public string InstrumentName { get; set; } = string.Empty;

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("marketAndExchangeNames")]
    public string MarketAndExchangeNames { get; set; } = string.Empty;

    [JsonPropertyName("reportDate")]
    public DateTime? ReportDate { get; set; }

    [JsonPropertyName("contractUnits")]
    public string ContractUnits { get; set; } = string.Empty;

    [JsonPropertyName("openInterestAll")]
    public int? OpenInterestAll { get; set; }

    [JsonPropertyName("changeInOpenInterestAll")]
    public int? ChangeInOpenInterestAll { get; set; }

    [JsonPropertyName("totalTraders")]
    public int? TotalTraders { get; set; }

    [JsonPropertyName("categories")]
    public List<CotLatestReportCategoryResponse> Categories { get; set; } = [];
}

public sealed class CotLatestReportCategoryResponse
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("long")]
    public CotLatestReportMetricResponse? Long { get; set; }

    [JsonPropertyName("short")]
    public CotLatestReportMetricResponse? Short { get; set; }

    [JsonPropertyName("spread")]
    public CotLatestReportMetricResponse? Spread { get; set; }
}

public sealed class CotLatestReportMetricResponse
{
    [JsonPropertyName("positions")]
    public int? Positions { get; set; }

    [JsonPropertyName("percentOfOpenInterest")]
    public decimal? PercentOfOpenInterest { get; set; }

    [JsonPropertyName("traders")]
    public int? Traders { get; set; }

    [JsonPropertyName("change")]
    public int? Change { get; set; }
}
