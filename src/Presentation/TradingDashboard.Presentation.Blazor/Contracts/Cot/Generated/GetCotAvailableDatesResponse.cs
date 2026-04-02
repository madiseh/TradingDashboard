using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Cot.Generated;

public sealed class GetCotAvailableDatesResponse
{
    [JsonPropertyName("availableDates")]
    public IReadOnlyList<string>? AvailableDates { get; set; }

    [JsonPropertyName("instrumentId")]
    public string? InstrumentId { get; set; }

    [JsonPropertyName("instrumentName")]
    public string? InstrumentName { get; set; }

    [JsonPropertyName("instrumentSlug")]
    public string? InstrumentSlug { get; set; }

    [JsonPropertyName("latestDate")]
    public string? LatestDate { get; set; }

    [JsonPropertyName("maxDate")]
    public string? MaxDate { get; set; }

    [JsonPropertyName("minDate")]
    public string? MinDate { get; set; }

    [JsonPropertyName("model")]
    public string? Model { get; set; }
}
