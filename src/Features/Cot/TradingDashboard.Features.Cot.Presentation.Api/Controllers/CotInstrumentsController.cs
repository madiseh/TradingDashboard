using Microsoft.AspNetCore.Mvc;
using MediatR;
using TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentBySlug;
using TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentsByGroup;
using TradingDashboard.Presentation.Common.Contracts;

namespace TradingDashboard.Presentation.Api.Cot.Controllers;

[ApiController]
[Route(RoutePaths.Api.Cot.CotInstruments)]
public class CotInstrumentsController(IMediator _mediator) : ControllerBase
{
    [HttpGet("{instrumentSlug}")]
    [ProducesResponseType(typeof(GetCotInstrumentBySlugResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBySlug([FromRoute] string instrumentSlug, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCotInstrumentBySlugQuery(instrumentSlug), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiErrorResponse(result.Message, result.ErrorCode, result.Errors));
        }

        return Ok(result.Data);
    }

    [HttpGet("by-group")]
    [ProducesResponseType(typeof(IReadOnlyList<GetCotInstrumentsByGroupResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetByGroup([FromQuery] string? group, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCotInstrumentsByGroupQuery(group), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new ApiErrorResponse(result.Message, result.ErrorCode, result.Errors));
        }

        return Ok(result.Data);
    }
}
