using MediatR;
using TradingDashboard.Application.Common.Models;
using TradingDashboard.Application.Common.Results;

namespace TradingDashboard.Application.Features.CotReports.Queries.GetCotReportSeries;

public sealed record GetCotReportSeriesQuery(
    string InstrumentSlug,
    CotReportModel Model,
    string Group,
    DateOnly FromDate,
    DateOnly ToDate)
    : IRequest<OperationalResult<GetCotReportSeriesResponse>>;
