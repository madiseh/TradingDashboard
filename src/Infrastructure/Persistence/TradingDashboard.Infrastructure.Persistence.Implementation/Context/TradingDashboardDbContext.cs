

namespace TradingDashboard.Infrastructure.Persistence.SimulatedData.Context;

public sealed class TradingDashboardDbContext : DbContext
{
    public TradingDashboardDbContext(DbContextOptions<TradingDashboardDbContext> options) : base(options)
    {
    }

    public DbSet<CotInstrument> CotInstruments => Set<CotInstrument>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new CotInstrumentConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
