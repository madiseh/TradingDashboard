using TradingDashboard.Application.Repository.Cot;
using TradingDashboard.Infrastructure.Persistence.Common.Repositories;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

public sealed class CotInstrumentRepository : GenericRepository<CotInstrument, Guid>, ICotInstrumentRepository
{
    public CotInstrumentRepository(DbContext context) : base(context)
    {
    }

    public CotInstrument? GetBySlug(string instrumentSlug)
    {
        if (string.IsNullOrWhiteSpace(instrumentSlug))
        {
            throw new ArgumentException("Instrument slug cannot be empty.", nameof(instrumentSlug));
        }

        return _context.Set<CotInstrument>()
            .FirstOrDefault(x => string.Equals(x.InstrumentSlug, instrumentSlug, StringComparison.OrdinalIgnoreCase));
    }
}
