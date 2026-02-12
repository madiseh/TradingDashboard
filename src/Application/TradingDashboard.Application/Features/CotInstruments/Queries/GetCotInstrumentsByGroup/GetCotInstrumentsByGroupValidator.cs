using FluentValidation;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentsByGroup;

public sealed class GetCotInstrumentsByGroupValidator : AbstractValidator<GetCotInstrumentsByGroupQuery>
{
    public GetCotInstrumentsByGroupValidator()
    {
        RuleFor(x => x.Group)
            .NotEmpty().WithMessage("Group is required.")
            .MinimumLength(3).WithMessage("Group must be at least 3 characters.")
            .Must(group => CotInstrumentTypeExtensions.TryParse(group, out _))
            .WithMessage(x => $"Unsupported group '{x.Group}'.");
    }
}
