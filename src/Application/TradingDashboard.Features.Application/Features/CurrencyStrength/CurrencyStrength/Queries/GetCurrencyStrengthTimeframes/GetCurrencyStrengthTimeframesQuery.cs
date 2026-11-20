using MediatR;
using TradingDashboard.Application.Common.Results;

namespace TradingDashboard.Application.Features.CurrencyStrength.Queries.GetCurrencyStrengthTimeframes;

public sealed record GetCurrencyStrengthTimeframesQuery()
    : IRequest<OperationalResult<GetCurrencyStrengthTimeframesResponse>>;
