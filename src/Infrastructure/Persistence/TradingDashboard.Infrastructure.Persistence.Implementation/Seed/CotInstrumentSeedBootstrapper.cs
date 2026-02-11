using TradingDashboard.Infrastructure.Persistence.Implementation.Context;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Seed;

public static class CotInstrumentSeedBootstrapper
{
    public static int SeedMissingInstruments(TradingDashboardDbContext dbContext)
    {
        var existingInstrumentIds = dbContext.CotInstruments
            .Select(x => x.InstrumentId)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingInstruments = CommonMarketSeedData.Markets
            .Where(x => !existingInstrumentIds.Contains(x.InstrumentId))
            .Select(x => new CotInstrument(Guid.NewGuid(), x.InstrumentId, x.Slug, x.Name))
            .ToArray();

        if (missingInstruments.Length == 0)
        {
            return 0;
        }

        dbContext.CotInstruments.AddRange(missingInstruments);
        return dbContext.SaveChanges();
    }
}
