using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Cot;

public sealed class GetCotInstrumentsByGroupResponse
{
    [JsonPropertyName("group")]
    public string? Group { get; set; }

    [JsonPropertyName("items")]
    public IReadOnlyList<GetCotInstrumentsByGroupItemResponse>? Items { get; set; }
}

