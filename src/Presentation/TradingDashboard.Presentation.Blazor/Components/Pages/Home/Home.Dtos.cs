namespace TradingDashboard.Presentation.Blazor.Components.Pages.Home;

public partial class Home
{
    private sealed record CotInstrumentsByGroupResponseDto(
        string Group,
        IReadOnlyList<CotInstrumentsByGroupItemDto> Items);

    private sealed record CotInstrumentsByGroupItemDto(
        Guid Id,
        string InstrumentId,
        string InstrumentSlug,
        string InstrumentName,
        string Type);
}
