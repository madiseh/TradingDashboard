using MediatR;
using TradingDashboard.Application.Abstractions.Persistence;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentBySlug;

public sealed class GetCotInstrumentBySlugHandler(ICotInstrumentRepository repository)
    : IRequestHandler<GetCotInstrumentBySlugQuery, OperationalResult<GetCotInstrumentBySlugResponse>>
{
    public async Task<OperationalResult<GetCotInstrumentBySlugResponse>> Handle(
        GetCotInstrumentBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var validationErrors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.InstrumentSlug))
        {
            validationErrors.Add("InstrumentSlug is required.");
        }
        else if (request.InstrumentSlug.Trim().Length < 3)
        {
            validationErrors.Add("InstrumentSlug must be at least 3 characters.");
        }

        if (validationErrors.Count > 0)
        {
            return OperationalResult<GetCotInstrumentBySlugResponse>.Fail(
                "Validation failed",
                "VALIDATION",
                validationErrors.ToArray());
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
