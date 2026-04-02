using MediatR;
using TradingDashboard.Application.Common.Models;
using TradingDashboard.Application.Common.Results;

namespace TradingDashboard.Application.Features.CotReports.Queries.GetCotAvailableDates;

public sealed record GetCotAvailableDatesQuery(string InstrumentSlug, CotReportModel Model)
    : IRequest<OperationalResult<GetCotAvailableDatesResponse>>;
