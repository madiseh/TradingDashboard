

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Context;

public sealed class CotDbContext : DbContext
{
    public CotDbContext(DbContextOptions<CotDbContext> options) : base(options)
    {
    }

    public DbSet<CotCollectionJobState> CotCollectionJobStates => Set<CotCollectionJobState>();
    public DbSet<CotInstrument> CotInstruments => Set<CotInstrument>();
    public DbSet<CotInstrumentCollectionProfile> CotInstrumentCollectionProfiles => Set<CotInstrumentCollectionProfile>();
    public DbSet<CotInstrumentTypeLookup> CotInstrumentTypes => Set<CotInstrumentTypeLookup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CotCollectionJobStateConfiguration());
        modelBuilder.ApplyConfiguration(new CotInstrumentTypeLookupConfiguration());
        modelBuilder.ApplyConfiguration(new CotInstrumentConfiguration());
        modelBuilder.ApplyConfiguration(new CotInstrumentCollectionProfileConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
