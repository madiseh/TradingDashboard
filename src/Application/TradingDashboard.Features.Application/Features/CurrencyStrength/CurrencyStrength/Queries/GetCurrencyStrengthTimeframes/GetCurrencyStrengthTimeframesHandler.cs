using MediatR;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Features.CurrencyStrength.Domain.Catalog;

namespace TradingDashboard.Application.Features.CurrencyStrength.Queries.GetCurrencyStrengthTimeframes;

public sealed class GetCurrencyStrengthTimeframesHandler
    : IRequestHandler<GetCurrencyStrengthTimeframesQuery, OperationalResult<GetCurrencyStrengthTimeframesResponse>>
{
    public Task<OperationalResult<GetCurrencyStrengthTimeframesResponse>> Handle(
        GetCurrencyStrengthTimeframesQuery request,
        CancellationToken cancellationToken)
    {
        var response = new GetCurrencyStrengthTimeframesResponse(
            CurrencyStrengthCatalog.All
                .OrderBy(item => item.SortOrder)
                .Select(item => new CurrencyStrengthTimeframeItemResponse(
                    item.Key,
                    item.Label,
                    item.Description,
                    item.SortOrder,
                    item.IsDefault))
                .ToArray());

        return Task.FromResult(OperationalResult<GetCurrencyStrengthTimeframesResponse>.Success(
            response,
            "Currency strength timeframes fetched successfully."));
    }
}
