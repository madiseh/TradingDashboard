using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Generated.Cot;

public sealed class GetCotInstrumentsByGroupResponse
{
    [JsonPropertyName("group")]
    public string? Group { get; set; }

    [JsonPropertyName("items")]
    public IReadOnlyList<GetCotInstrumentsByGroupItemResponse>? Items { get; set; }
}

