using FluentValidation;

namespace TradingDashboard.Application.Features.CotReports.Queries.GetCotReportSeries;

public sealed class GetCotReportSeriesValidator : AbstractValidator<GetCotReportSeriesQuery>
{
    public GetCotReportSeriesValidator()
    {
        RuleFor(x => x.InstrumentSlug)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Group)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x)
            .Must(x => x.FromDate <= x.ToDate)
            .WithMessage("'from' date must be less than or equal to 'to' date.");
    }
}
