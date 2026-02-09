namespace TradingDashboard.Infrastructure.Persistence.Implementation.Seed;

public static class CommonMarketSeedData
{
    public static IReadOnlyCollection<MarketDirectorySeedItem> Markets =>
    [
        new("aud-cme-futures", "australian-dollar", "AUSTRALIAN DOLLAR", "Currencies", ["CotLegacy", "Cot"]),
        new("gbp-cme-futures", "british-pound-sterling", "BRITISH POUND STERLING", "Currencies", ["CotLegacy", "Cot"]),
        new("cad-cme-futures", "canadian-dollar", "CANADIAN DOLLAR", "Currencies", ["CotLegacy", "Cot"]),
        new("eur-cme-futures", "euro-fx", "EURO FX", "Currencies", ["CotLegacy", "Cot"]),
        new("jpy-cme-futures", "japanese-yen", "JAPANESE YEN", "Currencies", ["CotLegacy", "Cot"]),
        new("corn-cbot-futures", "corn", "CORN", "Grains", ["CotLegacy", "Cot"]),
        new("soybeans-cbot-futures", "soybeans", "SOYBEANS", "Grains", ["CotLegacy", "Cot"]),
        new("soybean-oil-cbot-futures", "soybean-oil", "SOYBEAN OIL", "Grains", ["CotLegacy", "Cot"]),
        new("wheat-srw-cbot-futures", "wheat-srw", "WHEAT-SRW", "Grains", ["CotLegacy", "Cot"]),
        new("canola-ice-futures", "canola", "CANOLA", "Grains", ["CotLegacy", "Cot"]),
        new("btc-cme-futures", "bitcoin", "BITCOIN", "Cryptocurrencies", ["Cot"]),
        new("eth-cme-futures", "ether", "ETHER", "Cryptocurrencies", ["Cot"])
    ];
}

public sealed record MarketDirectorySeedItem(
    string InstrumentId,
    string Slug,
    string Name,
    string Type,
    string[] AvailableModels);
