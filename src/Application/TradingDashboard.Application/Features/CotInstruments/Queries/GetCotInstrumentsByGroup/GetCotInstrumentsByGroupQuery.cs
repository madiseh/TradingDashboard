using MediatR;
using TradingDashboard.Application.Common.Results;

namespace TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentsByGroup;

public sealed record GetCotInstrumentsByGroupQuery(string Group)
    : IRequest<OperationalResult<GetCotInstrumentsByGroupResponse>>;
