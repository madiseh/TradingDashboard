using TradingDashboard.Domain.Common.Common;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Domain.Entities;

public sealed class CotInstrument : BaseEntity<Guid>
{
    public string InstrumentId { get; private set; }
    public string InstrumentSlug { get; private set; }
    public string InstrumentName { get; private set; }
    public CotInstrumentType Type { get; private set; }
    public CotInstrumentTypeLookup? TypeLookup { get; private set; }

    public CotInstrument(Guid id, string instrumentId, string instrumentSlug, string instrumentName, CotInstrumentType type) : base(id)
    {
        InstrumentId = string.IsNullOrWhiteSpace(instrumentId)
            ? throw new ArgumentException("Instrument id cannot be empty.", nameof(instrumentId))
            : instrumentId;

        InstrumentSlug = string.IsNullOrWhiteSpace(instrumentSlug)
            ? throw new ArgumentException("Instrument slug cannot be empty.", nameof(instrumentSlug))
            : instrumentSlug;

        InstrumentName = string.IsNullOrWhiteSpace(instrumentName)
            ? throw new ArgumentException("Instrument name cannot be empty.", nameof(instrumentName))
            : instrumentName;

        if (!Enum.IsDefined(type))
        {
            throw new ArgumentOutOfRangeException(nameof(type), "Instrument type is not valid.");
        }

        Type = type;
    }

    private CotInstrument() : base(Guid.Empty)
    {
        InstrumentId = string.Empty;
        InstrumentSlug = string.Empty;
        InstrumentName = string.Empty;
        Type = CotInstrumentType.Currencies;
    }
}

