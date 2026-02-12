using Microsoft.AspNetCore.Mvc;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Presentation.Api.Cot.Controllers;

[ApiController]
[Route(RoutePaths.Api.CotInstruments)]
public class CotInstrumentsController(ICotInstrumentQueryRepository repository) : ControllerBase
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
            type = instrument.Type.ToContractValue()
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

        if (!CotInstrumentTypeExtensions.TryParse(group, out var instrumentType))
        {
            return BadRequest(new
            {
                error = new
                {
                    code = "INVALID_GROUP",
                    message = $"Unsupported group '{group}'."
                }
            });
        }

        var items = repository.GetByGroup(instrumentType)
            .Select(instrument => new
            {
                id = instrument.Id,
                instrumentId = instrument.InstrumentId,
                instrumentSlug = instrument.InstrumentSlug,
                instrumentName = instrument.InstrumentName,
                type = instrument.Type.ToContractValue()
            })
            .ToArray();

        return Ok(new
        {
            group = instrumentType.ToContractValue(),
            items
        });
    }
}
