using TradingDashboard.Application.Common.Models;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Seed;

public static class CommonMarketSeedData
{
    private static readonly string[] BothModels =
    [
        CotReportModel.CotLegacy.ToContractValue(),
        CotReportModel.Cot.ToContractValue()
    ];

    public static IReadOnlyCollection<MarketDirectorySeedItem> Markets =>
    [
        #region Currencies
        new("aud-cme-futures", "australian-dollar", "AUSTRALIAN DOLLAR", CotInstrumentType.Currencies, BothModels),
        new("gbp-cme-futures", "british-pound-sterling", "BRITISH POUND STERLING", CotInstrumentType.Currencies, BothModels),
        new("cad-cme-futures", "canadian-dollar", "CANADIAN DOLLAR", CotInstrumentType.Currencies, BothModels),
        new("eur-cme-futures", "euro-fx", "EURO FX", CotInstrumentType.Currencies, BothModels),
        new("jpy-cme-futures", "japanese-yen", "JAPANESE YEN", CotInstrumentType.Currencies, BothModels),
        new("chf-cme-futures", "swiss-franc", "SWISS FRANC", CotInstrumentType.Currencies, BothModels),
        new("usd-index-ice-futures", "us-dollar-index", "U.S. DOLLAR INDEX", CotInstrumentType.Currencies, BothModels),
        new("mxn-cme-futures", "mexican-peso", "MEXICAN PESO", CotInstrumentType.Currencies, BothModels),
        new("nzd-cme-futures", "new-zealand-dollar", "NEW ZEALAND DOLLAR", CotInstrumentType.Currencies, BothModels),
        new("brl-cme-futures", "brazilian-real", "BRAZILIAN REAL", CotInstrumentType.Currencies, BothModels),
        new("zar-cme-futures", "south-african-rand", "SOUTH AFRICAN RAND", CotInstrumentType.Currencies, BothModels),
        #endregion

        #region Cryptocurrencies
        new("btc-cme-futures", "bitcoin", "BITCOIN", CotInstrumentType.Cryptocurrencies, BothModels),
        new("mbt-cme-futures", "micro-bitcoin", "MICRO BITCOIN", CotInstrumentType.Cryptocurrencies, BothModels),
        new("eth-cme-futures", "ether-cash-settled", "ETHER CASH SETTLED", CotInstrumentType.Cryptocurrencies, BothModels),
        new("met-cme-futures", "micro-ether", "MICRO ETHER", CotInstrumentType.Cryptocurrencies, BothModels),
        #endregion

        #region Indexes
        new("spx-cme-futures", "sp-500-stock-index", "S&P 500 STOCK INDEX", CotInstrumentType.Indexes, BothModels),
        new("ndx-mini-cme-futures", "nasdaq-100-stock-index-mini", "NASDAQ-100 STOCK INDEX (MINI)", CotInstrumentType.Indexes, BothModels),
        new("djia-cbot-futures", "dow-jones-industrial-avg-x5", "DOW JONES INDUSTRIAL AVG- x $5", CotInstrumentType.Indexes, BothModels),
        new("russell-2000-mini-ice-futures", "russell-2000-mini-index-future", "RUSSELL 2000 MINI INDEX FUTURE", CotInstrumentType.Indexes, BothModels),
        new("sp400-mini-cme-futures", "e-mini-sp-400-stock-index", "E-MINI S&P 400 STOCK INDEX", CotInstrumentType.Indexes, BothModels),
        new("sp500-mini-cme-futures", "e-mini-sp-500-stock-index", "E-MINI S&P 500 STOCK INDEX", CotInstrumentType.Indexes, BothModels),
        new("vix-cboe-futures", "vix-futures", "VIX FUTURES", CotInstrumentType.Indexes, BothModels),
        #endregion

        #region TreasuriesAndRates
        new("fed-funds-30d-cbot", "30-day-federal-funds", "30-DAY FEDERAL FUNDS", CotInstrumentType.TreasuriesAndRates, BothModels),
        new("us-2y-note-cbot", "2-year-us-treasury-notes", "2-YEAR U.S. TREASURY NOTES", CotInstrumentType.TreasuriesAndRates, BothModels),
        new("us-5y-note-cbot", "5-year-us-treasury-notes", "5-YEAR U.S. TREASURY NOTES", CotInstrumentType.TreasuriesAndRates, BothModels),
        new("us-10y-note-cbot", "10-year-us-treasury-notes", "10-YEAR U.S. TREASURY NOTES", CotInstrumentType.TreasuriesAndRates, BothModels),
        #endregion

        #region Energies
        new("wti-nymex-futures", "crude-oil-light-sweet", "CRUDE OIL, LIGHT SWEET", CotInstrumentType.Energies, BothModels),
        new("rbob-nymex-futures", "gasoline-blendstock-rbob", "GASOLINE BLENDSTOCK (RBOB)", CotInstrumentType.Energies, BothModels),
        new("ulsd-nymex-futures", "heating-oil-ny-harbor-ulsd", "#2 HEATING OIL, NY HARBOR-ULSD", CotInstrumentType.Energies, BothModels),
        new("natgas-nymex-futures", "natural-gas", "NATURAL GAS", CotInstrumentType.Energies, BothModels),
        #endregion

        #region Grains
        new("corn-cbot-futures", "corn", "CORN", CotInstrumentType.Grains, BothModels),
        new("soybeans-cbot-futures", "soybeans", "SOYBEANS", CotInstrumentType.Grains, BothModels),
        new("soybean-oil-cbot-futures", "soybean-oil", "SOYBEAN OIL", CotInstrumentType.Grains, BothModels),
        new("soybean-meal-cbot-futures", "soybean-meal", "SOYBEAN MEAL", CotInstrumentType.Grains, BothModels),
        new("wheat-srw-cbot-futures", "wheat-srw", "WHEAT-SRW", CotInstrumentType.Grains, BothModels),
        new("wheat-hrw-kcbt-futures", "wheat-hrw", "WHEAT-HRW", CotInstrumentType.Grains, BothModels),
        new("wheat-hrspring-mgex-futures", "wheat-hrspring", "WHEAT-HRSpring", CotInstrumentType.Grains, BothModels),
        new("rough-rice-cbot-futures", "rough-rice", "ROUGH RICE", CotInstrumentType.Grains, BothModels),
        new("canola-ice-futures", "canola", "CANOLA", CotInstrumentType.Grains, BothModels),
        #endregion

        #region LivestockAndDairy
        new("live-cattle-cme-futures", "live-cattle", "LIVE CATTLE", CotInstrumentType.LivestockAndDairy, BothModels),
        new("feeder-cattle-cme-futures", "feeder-cattle", "FEEDER CATTLE", CotInstrumentType.LivestockAndDairy, BothModels),
        new("lean-hogs-cme-futures", "lean-hogs", "LEAN HOGS", CotInstrumentType.LivestockAndDairy, BothModels),
        new("milk-class-iii-cme-futures", "milk-class-iii", "MILK, Class III", CotInstrumentType.LivestockAndDairy, BothModels),
        new("non-fat-dry-milk-cme-futures", "non-fat-dry-milk", "NON FAT DRY MILK", CotInstrumentType.LivestockAndDairy, BothModels),
        new("butter-cash-settled-cme-futures", "butter-cash-settled", "BUTTER (CASH SETTLED)", CotInstrumentType.LivestockAndDairy, BothModels),
        new("cheese-cash-settled-cme-futures", "cheese-cash-settled", "CHEESE (CASH-SETTLED)", CotInstrumentType.LivestockAndDairy, BothModels),
        #endregion

        #region Metals
        new("gold-comex-futures", "gold", "GOLD", CotInstrumentType.Metals, BothModels),
        new("silver-comex-futures", "silver", "SILVER", CotInstrumentType.Metals, BothModels),
        new("copper-grade-1-comex-futures", "copper-grade-1", "COPPER-GRADE #1", CotInstrumentType.Metals, BothModels),
        new("palladium-nymex-futures", "palladium", "PALLADIUM", CotInstrumentType.Metals, BothModels),
        new("platinum-nymex-futures", "platinum", "PLATINUM", CotInstrumentType.Metals, BothModels),
        new("aluminum-mwp-cme-futures", "aluminum-mwp", "ALUMINUM MWP", CotInstrumentType.Metals, BothModels),
        #endregion

        #region Softs
        new("cocoa-ice-futures", "cocoa", "COCOA", CotInstrumentType.Softs, BothModels),
        new("cotton-no-2-ice-futures", "cotton-no-2", "COTTON NO. 2", CotInstrumentType.Softs, BothModels),
        new("coffee-c-ice-futures", "coffee-c", "COFFEE C", CotInstrumentType.Softs, BothModels),
        new("sugar-no-11-ice-futures", "sugar-no-11", "SUGAR NO. 11", CotInstrumentType.Softs, BothModels),
        new("fcoj-ice-futures", "frzn-concentrated-orange-juice", "FRZN CONCENTRATED ORANGE JUICE", CotInstrumentType.Softs, BothModels),
        new("random-length-lumber-cme-futures", "random-length-lumber", "RANDOM LENGTH LUMBER", CotInstrumentType.Softs, BothModels)
        #endregion
    ];
}

public sealed record MarketDirectorySeedItem(
    string InstrumentId,
    string Slug,
    string Name,
    CotInstrumentType Type,
    string[] AvailableModels);

