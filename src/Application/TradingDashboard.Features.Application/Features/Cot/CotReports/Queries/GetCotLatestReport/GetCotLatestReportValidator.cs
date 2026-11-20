using FluentValidation;

namespace TradingDashboard.Application.Features.CotReports.Queries.GetCotLatestReport;

public sealed class GetCotLatestReportValidator : AbstractValidator<GetCotLatestReportQuery>
{
    public GetCotLatestReportValidator()
    {
        RuleFor(x => x.InstrumentSlug)
            .NotEmpty()
            .MaximumLength(200);
    }
}
