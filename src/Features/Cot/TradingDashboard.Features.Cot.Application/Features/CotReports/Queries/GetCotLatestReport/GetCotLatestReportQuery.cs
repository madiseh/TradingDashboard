using MediatR;
using TradingDashboard.Application.Common.Models;
using TradingDashboard.Application.Common.Results;

namespace TradingDashboard.Application.Features.CotReports.Queries.GetCotLatestReport;

public sealed record GetCotLatestReportQuery(string InstrumentSlug, CotReportModel Model, DateOnly? ReportDate = null)
    : IRequest<OperationalResult<GetCotLatestReportResponse>>;
