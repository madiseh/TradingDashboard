using FluentValidation;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentsByGroup;

public sealed class GetCotInstrumentsByGroupValidator : AbstractValidator<GetCotInstrumentsByGroupQuery>
{
    public GetCotInstrumentsByGroupValidator()
    {
        RuleFor(x => x.Group)
            .MinimumLength(3).When(x => !string.IsNullOrWhiteSpace(x.Group))
            .WithMessage("Group must be at least 3 characters.")
            .Must(group => string.IsNullOrWhiteSpace(group) || CotInstrumentTypeExtensions.TryParse(group, out _))
            .WithMessage(x => $"Unsupported group '{x.Group}'.");
    }
}
