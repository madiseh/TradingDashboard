namespace TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentBySlug;

public sealed record GetCotInstrumentBySlugResponse(
    Guid Id,
    string InstrumentId,
    string InstrumentSlug,
    string InstrumentName,
    string Type);
