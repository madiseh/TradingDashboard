namespace TradingDashboard.Infrastructure.Persistence.SimulatedData.Repositories;

public sealed class CotInstrumentFakeRepository : ICotInstrumentRepository
{
    private readonly List<CotInstrument> _instruments;

    public CotInstrumentFakeRepository()
    {
        _instruments = new List<CotInstrument>
        {
            new CotInstrument(Guid.NewGuid(), "01", "australian-dollar", "Australian Dollar"),
            new CotInstrument(Guid.NewGuid(), "02", "canadian-dollar", "Canadian Dollar"),
            new CotInstrument(Guid.NewGuid(), "03", "crude-oil-wti", "Crude Oil WTI"),
            new CotInstrument(Guid.NewGuid(), "04", "gold", "Gold"),
            new CotInstrument(Guid.NewGuid(), "05", "silver", "Silver"),
            new CotInstrument(Guid.NewGuid(), "06", "sp500", "S&P 500"),
            new CotInstrument(Guid.NewGuid(), "07", "euro-fx", "Euro FX"),
            new CotInstrument(Guid.NewGuid(), "08", "japanese-yen", "Japanese Yen"),
        };
    }

    public void Create(CotInstrument entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        _instruments.Add(entity);
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

    public void SaveChanges()
    {
        // No-op for fake repository
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
}
