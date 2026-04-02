namespace TradingDashboard.Application.Features.CotReports.Queries.GetCotAvailableDates;

public sealed record GetCotAvailableDatesResponse(
    Guid InstrumentId,
    string InstrumentSlug,
    string InstrumentName,
    string Model,
    DateOnly LatestDate,
    DateOnly MinDate,
    DateOnly MaxDate,
    IReadOnlyList<DateOnly> AvailableDates);
