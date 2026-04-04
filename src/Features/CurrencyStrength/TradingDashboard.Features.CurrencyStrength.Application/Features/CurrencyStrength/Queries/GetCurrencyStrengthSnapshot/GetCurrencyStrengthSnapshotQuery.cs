using MediatR;
using TradingDashboard.Application.Common.Results;

namespace TradingDashboard.Application.Features.CurrencyStrength.Queries.GetCurrencyStrengthSnapshot;

public sealed record GetCurrencyStrengthSnapshotQuery(string TimeframeKey)
    : IRequest<OperationalResult<GetCurrencyStrengthSnapshotResponse>>;
