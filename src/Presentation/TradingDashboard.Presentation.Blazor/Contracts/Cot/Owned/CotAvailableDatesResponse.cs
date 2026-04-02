using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Cot.Owned;

public sealed class CotAvailableDatesResponse
{
    [JsonPropertyName("instrumentId")]
    public Guid InstrumentId { get; set; }

    [JsonPropertyName("instrumentSlug")]
    public string InstrumentSlug { get; set; } = string.Empty;

    [JsonPropertyName("instrumentName")]
    public string InstrumentName { get; set; } = string.Empty;

    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("latestDate")]
    public DateOnly LatestDate { get; set; }

    [JsonPropertyName("minDate")]
    public DateOnly MinDate { get; set; }

    [JsonPropertyName("maxDate")]
    public DateOnly MaxDate { get; set; }

    [JsonPropertyName("availableDates")]
    public List<DateOnly> AvailableDates { get; set; } = [];
}
