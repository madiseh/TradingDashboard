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

        var normalizedSlug = instrumentSlug.Trim();

        return _context.Set<CotInstrument>()
            .FirstOrDefault(x => x.InstrumentSlug == normalizedSlug);
    }

    public IList<CotInstrument> GetByGroup(string group)
    {
        if (string.IsNullOrWhiteSpace(group))
        {
            throw new ArgumentException("Group cannot be empty.", nameof(group));
        }

        var normalizedGroup = group.Trim();

        return _context.Set<CotInstrument>()
            .Where(x => x.Type == normalizedGroup)
            .OrderBy(x => x.InstrumentName)
            .ToList();
    }
}
