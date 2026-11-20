using FluentValidation;
using MediatR;
using TradingDashboard.Application.Common.Models;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Repository.Cot;

namespace TradingDashboard.Application.Features.CotReports.Queries.GetCotAvailableDates;

public sealed class GetCotAvailableDatesHandler(
    ICotInstrumentQueryRepository instrumentRepository,
    ICotHistoricalReportQueryRepository historicalReportRepository,
    IValidator<GetCotAvailableDatesQuery> validator)
    : IRequestHandler<GetCotAvailableDatesQuery, OperationalResult<GetCotAvailableDatesResponse>>
{
    public async Task<OperationalResult<GetCotAvailableDatesResponse>> Handle(
        GetCotAvailableDatesQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationalResult<GetCotAvailableDatesResponse>.Fail(
                "Validation failed",
                "VALIDATION",
                validationResult.Errors
                    .Select(x => x.ErrorMessage)
                    .Distinct()
                    .ToArray());
        }

        try
        {
            var instrument = await instrumentRepository.GetBySlugAsync(request.InstrumentSlug, cancellationToken);
            if (instrument is null)
            {
                return OperationalResult<GetCotAvailableDatesResponse>.Fail(
                    "Not found",
                    "NOT_FOUND",
                    $"Instrument '{request.InstrumentSlug}' was not found.");
            }

            var availableDates = await historicalReportRepository.GetAvailableDatesAsync(
                request.InstrumentSlug,
                request.Model,
                cancellationToken);

            if (availableDates.Count == 0)
            {
                return OperationalResult<GetCotAvailableDatesResponse>.Fail(
                    "Collected report dates not found",
                    "NOT_FOUND",
                    $"No collected {ToUiModelValue(request.Model)} dates were found for '{request.InstrumentSlug}'.");
            }

            var orderedDates = availableDates
                .Distinct()
                .OrderBy(x => x)
                .ToArray();

            var response = new GetCotAvailableDatesResponse(
                instrument.Id,
                instrument.InstrumentSlug,
                instrument.InstrumentName,
                ToUiModelValue(request.Model),
                orderedDates[^1],
                orderedDates[0],
                orderedDates[^1],
                orderedDates);

            return OperationalResult<GetCotAvailableDatesResponse>.Success(response, "Collected report dates fetched successfully.");
        }
        catch (Exception ex)
        {
            return OperationalResult<GetCotAvailableDatesResponse>.Fail(
                "Unexpected error",
                "UNEXPECTED",
                ex.Message);
        }
    }

    private static string ToUiModelValue(CotReportModel model)
    {
        return model == CotReportModel.CotLegacy ? "cot-legacy" : "cot";
    }
}
