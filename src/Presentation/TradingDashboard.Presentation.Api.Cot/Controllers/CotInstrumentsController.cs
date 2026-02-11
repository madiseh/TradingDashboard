using Microsoft.AspNetCore.Mvc;

namespace TradingDashboard.Presentation.Api.Cot.Controllers;

[ApiController]
[Route(RoutePaths.Api.CotInstruments)]
public class CotInstrumentsController(ICotInstrumentRepository repository) : ControllerBase
{
    [HttpGet("{instrumentSlug}")]
    public IActionResult GetBySlug([FromRoute] string instrumentSlug)
    {
        var instrument = repository.GetBySlug(instrumentSlug);
        if (instrument is null)
        {
            return NotFound(new
            {
                error = new
                {
                    code = "INSTRUMENT_NOT_FOUND",
                    message = $"Instrument '{instrumentSlug}' was not found."
                }
            });
        }

        return Ok(new
        {
            id = instrument.Id,
            instrumentId = instrument.InstrumentId,
            instrumentSlug = instrument.InstrumentSlug,
            instrumentName = instrument.InstrumentName,
            type = instrument.Type
        });
    }

    [HttpGet("by-group")]
    public IActionResult GetByGroup([FromQuery] string group)
    {
        if (string.IsNullOrWhiteSpace(group))
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "INVALID_GROUP",
                    message = "Query parameter 'group' is required."
                }
            });
        }

        var items = repository.GetByGroup(group)
            .Select(instrument => new
            {
                id = instrument.Id,
                instrumentId = instrument.InstrumentId,
                instrumentSlug = instrument.InstrumentSlug,
                instrumentName = instrument.InstrumentName,
                type = instrument.Type
            })
            .ToArray();

        return Ok(new
        {
            group = group.Trim(),
            items
        });
    }
}
