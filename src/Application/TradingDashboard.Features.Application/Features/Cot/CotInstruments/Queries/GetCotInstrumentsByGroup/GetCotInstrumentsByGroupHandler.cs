using FluentValidation;
using MediatR;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Repository.Cot;
using TradingDashboard.Domain.Entities;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentsByGroup;

public sealed class GetCotInstrumentsByGroupHandler(
    ICotInstrumentQueryRepository repository,
    IValidator<GetCotInstrumentsByGroupQuery> validator)
    : IRequestHandler<GetCotInstrumentsByGroupQuery, OperationalResult<IReadOnlyList<GetCotInstrumentsByGroupResponse>>>
{
    public async Task<OperationalResult<IReadOnlyList<GetCotInstrumentsByGroupResponse>>> Handle(
        GetCotInstrumentsByGroupQuery request,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return OperationalResult<IReadOnlyList<GetCotInstrumentsByGroupResponse>>.Fail(
                "Validation failed",
                "VALIDATION",
                validationResult.Errors
                    .Select(x => x.ErrorMessage)
                    .Distinct()
                    .ToArray());
        }

        try
        {
            if (!string.IsNullOrWhiteSpace(request.Group))
            {
                return await HandleSingleGroupAsync(request.Group, cancellationToken);
            }

            var allInstruments = await repository.GetAllOrderedAsync(cancellationToken);
            var groupedResponse = BuildGroupedResponse(allInstruments);

            if (groupedResponse.Count == 0)
            {
                return OperationalResult<IReadOnlyList<GetCotInstrumentsByGroupResponse>>.Fail(
                    "Not found",
                    "NOT_FOUND",
                    "No instruments found.");
            }

            return OperationalResult<IReadOnlyList<GetCotInstrumentsByGroupResponse>>.Success(
                groupedResponse,
                "Instruments fetched successfully.");
        }
        catch (Exception ex)
        {
            return OperationalResult<IReadOnlyList<GetCotInstrumentsByGroupResponse>>.Fail(
                "Unexpected error",
                "UNEXPECTED",
                ex.Message);
        }
    }

    private async Task<OperationalResult<IReadOnlyList<GetCotInstrumentsByGroupResponse>>> HandleSingleGroupAsync(
        string group,
        CancellationToken cancellationToken)
    {
        if (!CotInstrumentTypeExtensions.TryParse(group, out var instrumentType))
        {
            return OperationalResult<IReadOnlyList<GetCotInstrumentsByGroupResponse>>.Fail(
                "Validation failed",
                "VALIDATION",
                $"Unsupported group '{group}'.");
        }

        var instruments = await repository.GetByGroupAsync(instrumentType, cancellationToken);

        if (instruments.Count == 0)
        {
            return OperationalResult<IReadOnlyList<GetCotInstrumentsByGroupResponse>>.Fail(
                "Not found",
                "NOT_FOUND",
                $"No instruments found for group '{instrumentType.ToContractValue()}'.");
        }

        return OperationalResult<IReadOnlyList<GetCotInstrumentsByGroupResponse>>.Success(
            BuildGroupedResponse(instruments),
            "Instruments fetched successfully.");
    }

    private static IReadOnlyList<GetCotInstrumentsByGroupResponse> BuildGroupedResponse(
        IReadOnlyList<CotInstrument> instruments)
    {
        return instruments
            .GroupBy(x => x.Type)
            .OrderBy(x => x.Key)
            .Select(group => new GetCotInstrumentsByGroupResponse(
                group.Key.ToContractValue(),
                group.Select(instrument => new GetCotInstrumentsByGroupItemResponse(
                        instrument.Id,
                        instrument.InstrumentId,
                        instrument.InstrumentSlug,
                        instrument.InstrumentName,
                        instrument.Type.ToContractValue()))
                    .ToArray()))
            .ToArray();
    }
}
