using FluentValidation;

namespace TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentBySlug;

public sealed class GetCotInstrumentBySlugValidator : AbstractValidator<GetCotInstrumentBySlugQuery>
{
    public GetCotInstrumentBySlugValidator()
    {
        RuleFor(x => x.InstrumentSlug)
            .NotEmpty().WithMessage("InstrumentSlug is required.")
            .MinimumLength(3).WithMessage("InstrumentSlug must be at least 3 characters.");
    }
}
