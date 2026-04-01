using TradingDashboard.Application.Common.Models;
using TradingDashboard.Infrastructure.Persistence.Implementation.Context;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Seed;

public static class CotInstrumentSeedBootstrapper
{
    public const string CollectorStateId = "cot-cftc-collector";

    public static int SeedMissingInstruments(CotDbContext dbContext)
    {
        var expectedTypes = Enum.GetValues<CotInstrumentType>()
            .Select(type => new CotInstrumentTypeLookup(type, type.ToContractValue()))
            .ToArray();

        var existingTypeIds = dbContext.CotInstrumentTypes
            .Select(x => x.Id)
            .ToHashSet();

        var missingTypes = expectedTypes
            .Where(x => !existingTypeIds.Contains(x.Id))
            .ToArray();

        if (missingTypes.Length > 0)
        {
            dbContext.CotInstrumentTypes.AddRange(missingTypes);
        }

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

        dbContext.SaveChanges();

        SeedCollectionProfiles(dbContext);
        SeedCollectorState(dbContext);

        return dbContext.SaveChanges();
    }

    private static void SeedCollectionProfiles(CotDbContext dbContext)
    {
        var now = DateTime.UtcNow;
        var seedLookup = CommonMarketSeedData.Markets
            .ToDictionary(x => x.InstrumentId, StringComparer.OrdinalIgnoreCase);

        var instrumentLookup = dbContext.CotInstruments
            .ToDictionary(x => x.InstrumentId, StringComparer.OrdinalIgnoreCase);

        var existingProfiles = dbContext.CotInstrumentCollectionProfiles.ToDictionary(x => x.Id);

        foreach (var instrument in instrumentLookup.Values)
        {
            if (!seedLookup.TryGetValue(instrument.InstrumentId, out var seedItem))
            {
                continue;
            }

            var normalName = BuildMarketAndExchangeName(seedItem.Name, seedItem.SourceExchangeName);
            var legacyName = BuildMarketAndExchangeName(seedItem.Name, seedItem.SourceExchangeName);
            var normalEnabled = seedItem.AvailableModels.Contains(CotReportModel.Cot.ToContractValue(), StringComparer.OrdinalIgnoreCase);
            var legacyEnabled = seedItem.AvailableModels.Contains(CotReportModel.CotLegacy.ToContractValue(), StringComparer.OrdinalIgnoreCase);

            if (existingProfiles.TryGetValue(instrument.Id, out var existingProfile))
            {
                existingProfile.Refresh(normalName, legacyName, normalEnabled, legacyEnabled, now);
                continue;
            }

            dbContext.CotInstrumentCollectionProfiles.Add(
                new CotInstrumentCollectionProfile(
                    instrument.Id,
                    normalName,
                    legacyName,
                    normalEnabled,
                    legacyEnabled,
                    now));
        }
    }

    private static void SeedCollectorState(CotDbContext dbContext)
    {
        var state = dbContext.CotCollectionJobStates.Find(CollectorStateId);
        if (state is not null)
        {
            return;
        }

        dbContext.CotCollectionJobStates.Add(new CotCollectionJobState(CollectorStateId, DateTime.UtcNow));
    }

    private static string BuildMarketAndExchangeName(string marketName, string exchangeName)
    {
        return $"{marketName.Trim()} - {exchangeName.Trim()}";
    }
}
