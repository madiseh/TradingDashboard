namespace TradingDashboard.Infrastructure.Persistence.Implementation.Seed;

public static class CommonMarketSeedData
{
    private static readonly string[] BothModels = ["CotLegacy", "Cot"];

    public static IReadOnlyCollection<MarketDirectorySeedItem> Markets =>
    [
        #region Currencies
        new("aud-cme-futures", "australian-dollar", "AUSTRALIAN DOLLAR", "Currencies", BothModels),
        new("gbp-cme-futures", "british-pound-sterling", "BRITISH POUND STERLING", "Currencies", BothModels),
        new("cad-cme-futures", "canadian-dollar", "CANADIAN DOLLAR", "Currencies", BothModels),
        new("eur-cme-futures", "euro-fx", "EURO FX", "Currencies", BothModels),
        new("jpy-cme-futures", "japanese-yen", "JAPANESE YEN", "Currencies", BothModels),
        new("chf-cme-futures", "swiss-franc", "SWISS FRANC", "Currencies", BothModels),
        new("usd-index-ice-futures", "us-dollar-index", "U.S. DOLLAR INDEX", "Currencies", BothModels),
        new("mxn-cme-futures", "mexican-peso", "MEXICAN PESO", "Currencies", BothModels),
        new("nzd-cme-futures", "new-zealand-dollar", "NEW ZEALAND DOLLAR", "Currencies", BothModels),
        new("brl-cme-futures", "brazilian-real", "BRAZILIAN REAL", "Currencies", BothModels),
        new("zar-cme-futures", "south-african-rand", "SOUTH AFRICAN RAND", "Currencies", BothModels),
        #endregion

        #region Cryptocurrencies
        new("btc-cme-futures", "bitcoin", "BITCOIN", "Cryptocurrencies", BothModels),
        new("mbt-cme-futures", "micro-bitcoin", "MICRO BITCOIN", "Cryptocurrencies", BothModels),
        new("eth-cme-futures", "ether-cash-settled", "ETHER CASH SETTLED", "Cryptocurrencies", BothModels),
        new("met-cme-futures", "micro-ether", "MICRO ETHER", "Cryptocurrencies", BothModels),
        #endregion

        #region Indexes
        new("spx-cme-futures", "sp-500-stock-index", "S&P 500 STOCK INDEX", "Indexes", BothModels),
        new("ndx-mini-cme-futures", "nasdaq-100-stock-index-mini", "NASDAQ-100 STOCK INDEX (MINI)", "Indexes", BothModels),
        new("djia-cbot-futures", "dow-jones-industrial-avg-x5", "DOW JONES INDUSTRIAL AVG- x $5", "Indexes", BothModels),
        new("russell-2000-mini-ice-futures", "russell-2000-mini-index-future", "RUSSELL 2000 MINI INDEX FUTURE", "Indexes", BothModels),
        new("sp400-mini-cme-futures", "e-mini-sp-400-stock-index", "E-MINI S&P 400 STOCK INDEX", "Indexes", BothModels),
        new("sp500-mini-cme-futures", "e-mini-sp-500-stock-index", "E-MINI S&P 500 STOCK INDEX", "Indexes", BothModels),
        new("vix-cboe-futures", "vix-futures", "VIX FUTURES", "Indexes", BothModels),
        #endregion

        #region TreasuriesAndRates
        new("fed-funds-30d-cbot", "30-day-federal-funds", "30-DAY FEDERAL FUNDS", "Treasuries and Rates", BothModels),
        new("us-2y-note-cbot", "2-year-us-treasury-notes", "2-YEAR U.S. TREASURY NOTES", "Treasuries and Rates", BothModels),
        new("us-5y-note-cbot", "5-year-us-treasury-notes", "5-YEAR U.S. TREASURY NOTES", "Treasuries and Rates", BothModels),
        new("us-10y-note-cbot", "10-year-us-treasury-notes", "10-YEAR U.S. TREASURY NOTES", "Treasuries and Rates", BothModels),
        #endregion

        #region Energies
        new("wti-nymex-futures", "crude-oil-light-sweet", "CRUDE OIL, LIGHT SWEET", "Energies", BothModels),
        new("rbob-nymex-futures", "gasoline-blendstock-rbob", "GASOLINE BLENDSTOCK (RBOB)", "Energies", BothModels),
        new("ulsd-nymex-futures", "heating-oil-ny-harbor-ulsd", "#2 HEATING OIL, NY HARBOR-ULSD", "Energies", BothModels),
        new("natgas-nymex-futures", "natural-gas", "NATURAL GAS", "Energies", BothModels),
        #endregion

        #region Grains
        new("corn-cbot-futures", "corn", "CORN", "Grains", BothModels),
        new("soybeans-cbot-futures", "soybeans", "SOYBEANS", "Grains", BothModels),
        new("soybean-oil-cbot-futures", "soybean-oil", "SOYBEAN OIL", "Grains", BothModels),
        new("soybean-meal-cbot-futures", "soybean-meal", "SOYBEAN MEAL", "Grains", BothModels),
        new("wheat-srw-cbot-futures", "wheat-srw", "WHEAT-SRW", "Grains", BothModels),
        new("wheat-hrw-kcbt-futures", "wheat-hrw", "WHEAT-HRW", "Grains", BothModels),
        new("wheat-hrspring-mgex-futures", "wheat-hrspring", "WHEAT-HRSpring", "Grains", BothModels),
        new("rough-rice-cbot-futures", "rough-rice", "ROUGH RICE", "Grains", BothModels),
        new("canola-ice-futures", "canola", "CANOLA", "Grains", BothModels),
        #endregion

        #region LivestockAndDairy
        new("live-cattle-cme-futures", "live-cattle", "LIVE CATTLE", "Livestock & Dairy", BothModels),
        new("feeder-cattle-cme-futures", "feeder-cattle", "FEEDER CATTLE", "Livestock & Dairy", BothModels),
        new("lean-hogs-cme-futures", "lean-hogs", "LEAN HOGS", "Livestock & Dairy", BothModels),
        new("milk-class-iii-cme-futures", "milk-class-iii", "MILK, Class III", "Livestock & Dairy", BothModels),
        new("non-fat-dry-milk-cme-futures", "non-fat-dry-milk", "NON FAT DRY MILK", "Livestock & Dairy", BothModels),
        new("butter-cash-settled-cme-futures", "butter-cash-settled", "BUTTER (CASH SETTLED)", "Livestock & Dairy", BothModels),
        new("cheese-cash-settled-cme-futures", "cheese-cash-settled", "CHEESE (CASH-SETTLED)", "Livestock & Dairy", BothModels),
        #endregion

        #region Metals
        new("gold-comex-futures", "gold", "GOLD", "Metals", BothModels),
        new("silver-comex-futures", "silver", "SILVER", "Metals", BothModels),
        new("copper-grade-1-comex-futures", "copper-grade-1", "COPPER-GRADE #1", "Metals", BothModels),
        new("palladium-nymex-futures", "palladium", "PALLADIUM", "Metals", BothModels),
        new("platinum-nymex-futures", "platinum", "PLATINUM", "Metals", BothModels),
        new("aluminum-mwp-cme-futures", "aluminum-mwp", "ALUMINUM MWP", "Metals", BothModels),
        #endregion

        #region Softs
        new("cocoa-ice-futures", "cocoa", "COCOA", "Softs", BothModels),
        new("cotton-no-2-ice-futures", "cotton-no-2", "COTTON NO. 2", "Softs", BothModels),
        new("coffee-c-ice-futures", "coffee-c", "COFFEE C", "Softs", BothModels),
        new("sugar-no-11-ice-futures", "sugar-no-11", "SUGAR NO. 11", "Softs", BothModels),
        new("fcoj-ice-futures", "frzn-concentrated-orange-juice", "FRZN CONCENTRATED ORANGE JUICE", "Softs", BothModels),
        new("random-length-lumber-cme-futures", "random-length-lumber", "RANDOM LENGTH LUMBER", "Softs", BothModels)
        #endregion
    ];
}

public sealed record MarketDirectorySeedItem(
    string InstrumentId,
    string Slug,
    string Name,
    string Type,
    string[] AvailableModels);
