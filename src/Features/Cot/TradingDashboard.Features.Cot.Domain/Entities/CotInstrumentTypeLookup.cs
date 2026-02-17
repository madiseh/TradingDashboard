using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Domain.Entities;

public sealed class CotInstrumentTypeLookup
{
    public CotInstrumentType Id { get; private set; }
    public string Name { get; private set; }

    public CotInstrumentTypeLookup(CotInstrumentType id, string name)
    {
        if (!Enum.IsDefined(id))
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Instrument type is not valid.");
        }

        Id = id;
        Name = string.IsNullOrWhiteSpace(name)
            ? throw new ArgumentException("Type name cannot be empty.", nameof(name))
            : name;
    }

    private CotInstrumentTypeLookup()
    {
        Id = CotInstrumentType.Currencies;
        Name = string.Empty;
    }
}
