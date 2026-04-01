using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Cot.Generated;

public sealed class GetCotInstrumentBySlugResponse
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    [JsonPropertyName("instrumentId")]
    public string? InstrumentId { get; set; }

    [JsonPropertyName("instrumentName")]
    public string? InstrumentName { get; set; }

    [JsonPropertyName("instrumentSlug")]
    public string? InstrumentSlug { get; set; }

    [JsonPropertyName("type")]
    public string? Type { get; set; }
}
