namespace TradingDashboard.Presentation.Blazor.Features.Common.Contracts;

public sealed class MarketsDirectoryResponse
{
    public IReadOnlyList<MarketGroupDto> Groups { get; set; } = [];
}

public sealed class MarketGroupDto
{
    public string Type { get; set; } = string.Empty;
    public IReadOnlyList<MarketItemDto> Items { get; set; } = [];
}

public sealed class MarketItemDto
{
    public string InstrumentId { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public IReadOnlyList<string> AvailableModels { get; set; } = [];
}
