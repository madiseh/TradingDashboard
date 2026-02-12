using FluentValidation;
using MediatR;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Repository.Cot;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentBySlug;

public sealed class GetCotInstrumentBySlugHandler(
    ICotInstrumentQueryRepository repository,
    IValidator<GetCotInstrumentBySlugQuery> validator)
    : IRequestHandler<GetCotInstrumentBySlugQuery, OperationalResult<GetCotInstrumentBySlugResponse>>
{
    public async Task<OperationalResult<GetCotInstrumentBySlugResponse>> Handle(
        GetCotInstrumentBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationalResult<GetCotInstrumentBySlugResponse>.Fail(
                "Validation failed",
                "VALIDATION",
                validationResult.Errors
                    .Select(x => x.ErrorMessage)
                    .Distinct()
                    .ToArray());
        }

        try
        {
            var instrument = await repository.GetBySlugAsync(request.InstrumentSlug, cancellationToken);
            if (instrument is null)
            {
                return OperationalResult<GetCotInstrumentBySlugResponse>.Fail(
                    "Not found",
                    "NOT_FOUND",
                    $"Instrument '{request.InstrumentSlug}' was not found.");
            }

            var response = new GetCotInstrumentBySlugResponse(
                instrument.Id,
                instrument.InstrumentId,
                instrument.InstrumentSlug,
                instrument.InstrumentName,
                instrument.Type.ToContractValue());

            return OperationalResult<GetCotInstrumentBySlugResponse>.Success(response, "Instrument fetched successfully.");
        }
        catch (Exception ex)
        {
            return OperationalResult<GetCotInstrumentBySlugResponse>.Fail(
                "Unexpected error",
                "UNEXPECTED",
                ex.Message);
        }
    }
}
