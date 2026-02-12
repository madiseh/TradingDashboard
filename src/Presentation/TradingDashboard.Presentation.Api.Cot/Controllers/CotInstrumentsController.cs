using Microsoft.AspNetCore.Mvc;
using MediatR;
using TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentBySlug;
using TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentsByGroup;

namespace TradingDashboard.Presentation.Api.Cot.Controllers;

[ApiController]
[Route(RoutePaths.Api.CotInstruments)]
public class CotInstrumentsController(IMediator _mediator) : ControllerBase
{
    [HttpGet("{instrumentSlug}")]
    public async Task<IActionResult> GetBySlug([FromRoute] string instrumentSlug, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCotInstrumentBySlugQuery(instrumentSlug), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                result.Message,
                result.ErrorCode,
                result.Errors
            });
        }

        return Ok(result.Data);
    }

    [HttpGet("by-group")]
    public async Task<IActionResult> GetByGroup([FromQuery] string? group, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetCotInstrumentsByGroupQuery(group), cancellationToken);
        if (!result.IsSuccess)
        {
            return BadRequest(new
            {
                result.Message,
                result.ErrorCode,
                result.Errors
            });
        }

        return Ok(result.Data);
    }
}
