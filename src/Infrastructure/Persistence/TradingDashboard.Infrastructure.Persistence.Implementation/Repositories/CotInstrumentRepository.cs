using TradingDashboard.Application.Abstractions.Persistence;
using TradingDashboard.Domain.Enums;
using TradingDashboard.Infrastructure.Persistence.Common.Repositories;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

public sealed class CotInstrumentRepository : GenericQueryRepository<CotInstrument, Guid>, ICotInstrumentRepository
{
    public CotInstrumentRepository(DbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsBySlugAsync(string instrumentSlug, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(instrumentSlug))
        {
            throw new ArgumentException("Instrument slug cannot be empty.", nameof(instrumentSlug));
        }

        var normalizedSlug = instrumentSlug.Trim();

        return await _context.Set<CotInstrument>()
            .AnyAsync(x => x.InstrumentSlug == normalizedSlug, cancellationToken);
    }

    public async Task<CotInstrument?> GetBySlugAsync(string instrumentSlug, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(instrumentSlug))
        {
            throw new ArgumentException("Instrument slug cannot be empty.", nameof(instrumentSlug));
        }

        var normalizedSlug = instrumentSlug.Trim();

        return await _context.Set<CotInstrument>()
            .FirstOrDefaultAsync(x => x.InstrumentSlug == normalizedSlug, cancellationToken);
    }

    public async Task<IReadOnlyList<CotInstrument>> GetByGroupAsync(CotInstrumentType group, CancellationToken cancellationToken)
    {
        return await _context.Set<CotInstrument>()
            .Where(x => x.Type == group)
            .OrderBy(x => x.InstrumentName)
            .ToListAsync(cancellationToken);
    }
}
