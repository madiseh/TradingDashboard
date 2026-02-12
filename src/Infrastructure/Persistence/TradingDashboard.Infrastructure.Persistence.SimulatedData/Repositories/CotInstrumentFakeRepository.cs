using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Infrastructure.Persistence.SimulatedData.Repositories;

public sealed class CotInstrumentFakeRepository : ICotInstrumentRepository
{
    private readonly List<CotInstrument> _instruments;

    public CotInstrumentFakeRepository()
    {
        _instruments = new List<CotInstrument>
        {
            new CotInstrument(Guid.NewGuid(), "01", "australian-dollar", "Australian Dollar", CotInstrumentType.Currencies),
            new CotInstrument(Guid.NewGuid(), "02", "canadian-dollar", "Canadian Dollar", CotInstrumentType.Currencies),
            new CotInstrument(Guid.NewGuid(), "03", "crude-oil-wti", "Crude Oil WTI", CotInstrumentType.Energies),
            new CotInstrument(Guid.NewGuid(), "04", "gold", "Gold", CotInstrumentType.Metals),
            new CotInstrument(Guid.NewGuid(), "05", "silver", "Silver", CotInstrumentType.Metals),
            new CotInstrument(Guid.NewGuid(), "06", "sp500", "S&P 500", CotInstrumentType.Indexes),
            new CotInstrument(Guid.NewGuid(), "07", "euro-fx", "Euro FX", CotInstrumentType.Currencies),
            new CotInstrument(Guid.NewGuid(), "08", "japanese-yen", "Japanese Yen", CotInstrumentType.Currencies),
        };
    }

    public CotInstrument Get(Guid id)
    {
        var instrument = _instruments.FirstOrDefault(x => x.Id == id);
        return instrument ?? throw new KeyNotFoundException($"Instrument with id '{id}' not found.");
    }

    public IList<CotInstrument> GetList()
    {
        return _instruments.ToList();
    }

    public bool Exists(System.Linq.Expressions.Expression<Func<CotInstrument, bool>> expression)
    {
        ArgumentNullException.ThrowIfNull(expression);
        return _instruments.AsQueryable().Any(expression);
    }

    public CotInstrument? GetBySlug(string instrumentSlug)
    {
        if (string.IsNullOrWhiteSpace(instrumentSlug))
        {
            throw new ArgumentException("Instrument slug cannot be empty.", nameof(instrumentSlug));
        }

        return _instruments.FirstOrDefault(x =>
            string.Equals(x.InstrumentSlug, instrumentSlug, StringComparison.OrdinalIgnoreCase));
    }

    public IList<CotInstrument> GetByGroup(CotInstrumentType group)
    {
        return _instruments
            .Where(x => x.Type == group)
            .OrderBy(x => x.InstrumentName)
            .ToList();
    }
}
