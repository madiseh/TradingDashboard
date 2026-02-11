using TradingDashboard.Infrastructure.Persistence.Implementation.Context;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Seed;

public static class CotInstrumentSeedBootstrapper
{
    public static int SeedMissingInstruments(TradingDashboardDbContext dbContext)
    {
        var seedLookup = CommonMarketSeedData.Markets
            .ToDictionary(x => x.InstrumentId, StringComparer.OrdinalIgnoreCase);

        var existingInstruments = dbContext.CotInstruments.ToList();
        foreach (var instrument in existingInstruments)
        {
            if (!seedLookup.TryGetValue(instrument.InstrumentId, out var seedItem))
            {
                continue;
            }

            dbContext.Entry(instrument).Property(nameof(CotInstrument.InstrumentSlug)).CurrentValue = seedItem.Slug;
            dbContext.Entry(instrument).Property(nameof(CotInstrument.InstrumentName)).CurrentValue = seedItem.Name;
            dbContext.Entry(instrument).Property(nameof(CotInstrument.Type)).CurrentValue = seedItem.Type;
        }

        var existingInstrumentIds = existingInstruments
            .Select(x => x.InstrumentId)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var missingInstruments = CommonMarketSeedData.Markets
            .Where(x => !existingInstrumentIds.Contains(x.InstrumentId))
            .Select(x => new CotInstrument(Guid.NewGuid(), x.InstrumentId, x.Slug, x.Name, x.Type))
            .ToArray();

        if (missingInstruments.Length > 0)
        {
            dbContext.CotInstruments.AddRange(missingInstruments);
        }

        return dbContext.SaveChanges();
    }
}
