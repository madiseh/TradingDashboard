using FluentValidation;
using MediatR;
using TradingDashboard.Application.Abstractions.Persistence;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentsByGroup;

public sealed class GetCotInstrumentsByGroupHandler(
    ICotInstrumentRepository repository,
    IValidator<GetCotInstrumentsByGroupQuery> validator)
    : IRequestHandler<GetCotInstrumentsByGroupQuery, OperationalResult<GetCotInstrumentsByGroupResponse>>
{
    public async Task<OperationalResult<GetCotInstrumentsByGroupResponse>> Handle(
        GetCotInstrumentsByGroupQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationalResult<GetCotInstrumentsByGroupResponse>.Fail(
                "Validation failed",
                "VALIDATION",
                validationResult.Errors
                    .Select(x => x.ErrorMessage)
                    .Distinct()
                    .ToArray());
        }

        if (!CotInstrumentTypeExtensions.TryParse(request.Group, out var instrumentType))
        {
            return OperationalResult<GetCotInstrumentsByGroupResponse>.Fail(
                "Validation failed",
                "VALIDATION",
                $"Unsupported group '{request.Group}'.");
        }

        try
        {
            var instruments = await repository.GetByGroupAsync(instrumentType, cancellationToken);

            var duplicateSlugs = instruments
                .GroupBy(x => x.InstrumentSlug, StringComparer.OrdinalIgnoreCase)
                .Where(x => x.Count() > 1)
                .Select(x => x.Key)
                .ToArray();

            if (duplicateSlugs.Length > 0)
            {
                return OperationalResult<GetCotInstrumentsByGroupResponse>.Fail(
                    "Already exists",
                    "DUPLICATE",
                    $"Duplicate instrument slug(s) detected: {string.Join(", ", duplicateSlugs)}");
            }

            if (instruments.Count == 0)
            {
                return OperationalResult<GetCotInstrumentsByGroupResponse>.Fail(
                    "Not found",
                    "NOT_FOUND",
                    $"No instruments found for group '{instrumentType.ToContractValue()}'.");
            }

            var response = new GetCotInstrumentsByGroupResponse(
                instrumentType.ToContractValue(),
                instruments
                    .Select(instrument => new GetCotInstrumentsByGroupItemResponse(
                        instrument.Id,
                        instrument.InstrumentId,
                        instrument.InstrumentSlug,
                        instrument.InstrumentName,
                        instrument.Type.ToContractValue()))
                    .ToArray());

            return OperationalResult<GetCotInstrumentsByGroupResponse>.Success(response, "Instruments fetched successfully.");
        }
        catch (Exception ex)
        {
            return OperationalResult<GetCotInstrumentsByGroupResponse>.Fail(
                "Unexpected error",
                "UNEXPECTED",
                ex.Message);
        }
    }
}
