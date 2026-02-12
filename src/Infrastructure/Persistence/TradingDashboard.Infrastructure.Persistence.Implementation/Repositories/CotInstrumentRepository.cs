using TradingDashboard.Application.Repository.Cot;
using TradingDashboard.Domain.Enums;
using TradingDashboard.Infrastructure.Persistence.Common.Repositories;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

public sealed class CotInstrumentRepository : GenericQueryRepository<CotInstrument, Guid>, ICotInstrumentRepository
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

    public IList<CotInstrument> GetByGroup(CotInstrumentType group)
    {
        return _context.Set<CotInstrument>()
            .Where(x => x.Type == group)
            .OrderBy(x => x.InstrumentName)
            .ToList();
    }
}
