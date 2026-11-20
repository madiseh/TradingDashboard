using FluentValidation;
using MediatR;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Repository.CurrencyStrength;
using TradingDashboard.Features.CurrencyStrength.Domain.Catalog;

namespace TradingDashboard.Application.Features.CurrencyStrength.Queries.GetCurrencyStrengthSnapshot;

public sealed class GetCurrencyStrengthSnapshotHandler(
    ICurrencyStrengthSnapshotQueryRepository snapshotRepository,
    IValidator<GetCurrencyStrengthSnapshotQuery> validator)
    : IRequestHandler<GetCurrencyStrengthSnapshotQuery, OperationalResult<GetCurrencyStrengthSnapshotResponse>>
{
    public async Task<OperationalResult<GetCurrencyStrengthSnapshotResponse>> Handle(
        GetCurrencyStrengthSnapshotQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationalResult<GetCurrencyStrengthSnapshotResponse>.Fail(
                "Validation failed",
                "VALIDATION",
                validationResult.Errors.Select(item => item.ErrorMessage).Distinct().ToArray());
        }

        CurrencyStrengthCatalog.TryGet(request.TimeframeKey, out var timeframe);

        var snapshot = await snapshotRepository.GetLatestAsync(request.TimeframeKey, cancellationToken);
        if (snapshot is null)
        {
            return OperationalResult<GetCurrencyStrengthSnapshotResponse>.Fail(
                "Not found",
                "NOT_FOUND",
                $"No collected snapshot was found for timeframe '{request.TimeframeKey}'.");
        }

        var lastPointAtUtc = snapshot.Series
            .SelectMany(series => series.Points)
            .Select(point => DateTimeOffset.FromUnixTimeMilliseconds(point.Timestamp).UtcDateTime)
            .DefaultIfEmpty()
            .Max();

        var response = new GetCurrencyStrengthSnapshotResponse(
            snapshot.TimeframeKey,
            snapshot.TimeframeLabel,
            timeframe.Description,
            snapshot.CollectedAtUtc,
            lastPointAtUtc == DateTime.MinValue ? null : lastPointAtUtc,
            snapshot.Series
                .Select(series => new CurrencyStrengthSeriesResponse(
                    series.CurrencyCode,
                    series.Points.Select(point => new CurrencyStrengthPointResponse(point.Timestamp, point.Value)).ToArray()))
                .ToArray());

        return OperationalResult<GetCurrencyStrengthSnapshotResponse>.Success(
            response,
            "Currency strength snapshot fetched successfully.");
    }
}
