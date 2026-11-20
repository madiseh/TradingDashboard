using MediatR;
using TradingDashboard.Application.Common.Results;

namespace TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentBySlug;

public sealed record GetCotInstrumentBySlugQuery(string InstrumentSlug)
    : IRequest<OperationalResult<GetCotInstrumentBySlugResponse>>;
