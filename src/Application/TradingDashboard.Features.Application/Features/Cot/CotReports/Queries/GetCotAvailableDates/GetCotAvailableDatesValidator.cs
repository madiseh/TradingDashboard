using FluentValidation;

namespace TradingDashboard.Application.Features.CotReports.Queries.GetCotAvailableDates;

public sealed class GetCotAvailableDatesValidator : AbstractValidator<GetCotAvailableDatesQuery>
{
    public GetCotAvailableDatesValidator()
    {
        RuleFor(x => x.InstrumentSlug)
            .NotEmpty()
            .MaximumLength(200);
    }
}
