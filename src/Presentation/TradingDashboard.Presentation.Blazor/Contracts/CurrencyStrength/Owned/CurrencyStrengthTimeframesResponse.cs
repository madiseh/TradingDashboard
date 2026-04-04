using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.CurrencyStrength.Owned;

public sealed class CurrencyStrengthTimeframesResponse
{
    [JsonPropertyName("items")]
    public List<CurrencyStrengthTimeframeItemResponse> Items { get; set; } = [];
}

public sealed class CurrencyStrengthTimeframeItemResponse
{
    [JsonPropertyName("key")]
    public string Key { get; set; } = string.Empty;

    [JsonPropertyName("label")]
    public string Label { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("sortOrder")]
    public int SortOrder { get; set; }

    [JsonPropertyName("isDefault")]
    public bool IsDefault { get; set; }
}
