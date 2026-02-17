

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Context;

public sealed class CotDbContext : DbContext
{
    public CotDbContext(DbContextOptions<CotDbContext> options) : base(options)
    {
    }

    public DbSet<CotInstrument> CotInstruments => Set<CotInstrument>();
    public DbSet<CotInstrumentTypeLookup> CotInstrumentTypes => Set<CotInstrumentTypeLookup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CotInstrumentTypeLookupConfiguration());
        modelBuilder.ApplyConfiguration(new CotInstrumentConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
