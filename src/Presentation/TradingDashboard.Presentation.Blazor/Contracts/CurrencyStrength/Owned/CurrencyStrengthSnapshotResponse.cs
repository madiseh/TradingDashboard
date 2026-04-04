using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.CurrencyStrength.Owned;

public sealed class CurrencyStrengthSnapshotResponse
{
    [JsonPropertyName("timeframeKey")]
    public string TimeframeKey { get; set; } = string.Empty;

    [JsonPropertyName("timeframeLabel")]
    public string TimeframeLabel { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("collectedAtUtc")]
    public DateTime CollectedAtUtc { get; set; }

    [JsonPropertyName("lastPointAtUtc")]
    public DateTime? LastPointAtUtc { get; set; }

    [JsonPropertyName("series")]
    public List<CurrencyStrengthSeriesResponse> Series { get; set; } = [];
}

public sealed class CurrencyStrengthSeriesResponse
{
    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; } = string.Empty;

    [JsonPropertyName("points")]
    public List<CurrencyStrengthPointResponse> Points { get; set; } = [];
}

public sealed class CurrencyStrengthPointResponse
{
    [JsonPropertyName("timestamp")]
    public long Timestamp { get; set; }

    [JsonPropertyName("value")]
    public decimal Value { get; set; }
}
