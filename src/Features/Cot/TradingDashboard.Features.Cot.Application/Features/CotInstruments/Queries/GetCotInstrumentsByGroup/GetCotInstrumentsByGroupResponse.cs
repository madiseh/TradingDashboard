namespace TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentsByGroup;

public sealed record GetCotInstrumentsByGroupResponse(
    string Group,
    IReadOnlyList<GetCotInstrumentsByGroupItemResponse> Items);

public sealed record GetCotInstrumentsByGroupItemResponse(
    Guid Id,
    string InstrumentId,
    string InstrumentSlug,
    string InstrumentName,
    string Type);
