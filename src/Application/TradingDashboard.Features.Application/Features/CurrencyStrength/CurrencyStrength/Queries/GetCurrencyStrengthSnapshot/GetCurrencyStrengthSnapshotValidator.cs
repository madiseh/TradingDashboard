using FluentValidation;
using TradingDashboard.Features.CurrencyStrength.Domain.Catalog;

namespace TradingDashboard.Application.Features.CurrencyStrength.Queries.GetCurrencyStrengthSnapshot;

public sealed class GetCurrencyStrengthSnapshotValidator : AbstractValidator<GetCurrencyStrengthSnapshotQuery>
{
    public GetCurrencyStrengthSnapshotValidator()
    {
        RuleFor(x => x.TimeframeKey)
            .NotEmpty()
            .WithMessage("Timeframe is required.")
            .Must(key => CurrencyStrengthCatalog.TryGet(key, out _))
            .WithMessage("Requested timeframe is not supported.");
    }
}
