namespace TradingDashboard.UnitTests.Features.CotReports.Queries;

public sealed class CotReportQueriesTests
{
    private static readonly CotReportModel[] AllReportModels = [CotReportModel.Cot, CotReportModel.CotLegacy];

    [Fact]
    public async Task GetCotAvailableDates_ReturnsOrderedDatesAndMetadata()
    {
        var instrument = CreateInstrument();
        var handler = new GetCotAvailableDatesHandler(
            new FakeInstrumentRepository(instrument),
            new FakeHistoricalReportRepository(
                [new DateOnly(2026, 03, 11), new DateOnly(2026, 03, 04), new DateOnly(2026, 03, 18)],
                []),
            new GetCotAvailableDatesValidator());

        var result = await handler.Handle(
            new GetCotAvailableDatesQuery(instrument.InstrumentSlug, CotReportModel.Cot),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(new DateOnly(2026, 03, 04), result.Data!.MinDate);
        Assert.Equal(new DateOnly(2026, 03, 18), result.Data.MaxDate);
        Assert.Equal(new DateOnly(2026, 03, 18), result.Data.LatestDate);
        Assert.Equal(
            [new DateOnly(2026, 03, 04), new DateOnly(2026, 03, 11), new DateOnly(2026, 03, 18)],
            result.Data.AvailableDates);
    }

    [Fact]
    public async Task GetCotReportSeries_ComputesNetPositionFromLongAndShort()
    {
        var instrument = CreateInstrument();
        var handler = new GetCotReportSeriesHandler(
            new FakeInstrumentRepository(instrument),
            new FakeHistoricalReportRepository(
                [new DateOnly(2026, 03, 04), new DateOnly(2026, 03, 11)],
                [
                    CreateRawRecord(
                        new DateOnly(2026, 03, 04),
                        new Dictionary<string, string?>
                        {
                            ["dealer_positions_long_all"] = "120",
                            ["dealer_positions_short_all"] = "80",
                            ["dealer_positions_spread_all"] = "10",
                            ["open_interest_all"] = "500"
                        }),
                    CreateRawRecord(
                        new DateOnly(2026, 03, 11),
                        new Dictionary<string, string?>
                        {
                            ["dealer_positions_long_all"] = "90",
                            ["dealer_positions_short_all"] = "150",
                            ["dealer_positions_spread_all"] = "4",
                            ["open_interest_all"] = "540"
                        })
                ]),
            new GetCotReportSeriesValidator());

        var result = await handler.Handle(
            new GetCotReportSeriesQuery(
                instrument.InstrumentSlug,
                CotReportModel.Cot,
                "dealer",
                new DateOnly(2026, 03, 04),
                new DateOnly(2026, 03, 11)),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(2, result.Data!.Items.Count);
        Assert.Equal(40, result.Data.Items[0].NetPosition);
        Assert.Equal(-60, result.Data.Items[1].NetPosition);
        Assert.Equal(10, result.Data.Items[0].Spread);
        Assert.Equal(540, result.Data.Items[1].OpenInterest);
    }

    [Fact]
    public async Task GetCotLatestReport_BuildsDisaggregatedCategoriesForGoldCotReport()
    {
        var instrument = CreateGoldInstrument();
        var handler = new GetCotLatestReportHandler(
            new FakeInstrumentRepository(instrument),
            new FakeLatestReportRepository(CreateRawRecord(
                new DateOnly(2026, 03, 18),
                CreateDisaggregatedFields())),
            new GetCotLatestReportValidator());

        var result = await handler.Handle(
            new GetCotLatestReportQuery(instrument.InstrumentSlug, CotReportModel.Cot, null),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("gold", result.Data!.InstrumentSlug);
        Assert.Collection(
            result.Data.Categories,
            category => Assert.Equal("producer-merchant", category.Key),
            category => Assert.Equal("swap-dealers", category.Key),
            category =>
            {
                Assert.Equal("managed-money", category.Key);
                Assert.Equal(310, category.Long?.Positions);
                Assert.Equal(140, category.Short?.Positions);
                Assert.Equal(170, category.Long?.Positions - category.Short?.Positions);
            },
            category => Assert.Equal("other-reportables", category.Key),
            category => Assert.Equal("nonreportable", category.Key));
    }

    [Fact]
    public async Task GetCotReportSeries_ComputesDisaggregatedManagedMoneySeries()
    {
        var instrument = CreateGoldInstrument();
        var handler = new GetCotReportSeriesHandler(
            new FakeInstrumentRepository(instrument),
            new FakeHistoricalReportRepository(
                [new DateOnly(2026, 03, 11), new DateOnly(2026, 03, 18)],
                [
                    CreateRawRecord(
                        new DateOnly(2026, 03, 11),
                        CreateDisaggregatedFields(managedMoneyLong: 250, managedMoneyShort: 300)),
                    CreateRawRecord(
                        new DateOnly(2026, 03, 18),
                        CreateDisaggregatedFields(managedMoneyLong: 310, managedMoneyShort: 140))
                ]),
            new GetCotReportSeriesValidator());

        var result = await handler.Handle(
            new GetCotReportSeriesQuery(
                instrument.InstrumentSlug,
                CotReportModel.Cot,
                "managed-money",
                new DateOnly(2026, 03, 11),
                new DateOnly(2026, 03, 18)),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal([-50, 170], result.Data!.Items.Select(x => x.NetPosition).ToArray());
    }

    [Fact]
    public async Task CotReportQueries_LoadLatestAndAvailableDatesForEveryCurrencyInBothModels()
    {
        var storageRoot = Path.Combine(Path.GetTempPath(), "trading-dashboard-cot-tests", Guid.NewGuid().ToString("N"));
        var reportDates = new[] { new DateOnly(2026, 03, 11), new DateOnly(2026, 03, 18) };
        var currencySeeds = CommonMarketSeedData.Markets
            .Where(x => x.Type == CotInstrumentType.Currencies)
            .OrderBy(x => x.Slug)
            .ToArray();
        var instruments = currencySeeds
            .Select(x => new CotInstrument(Guid.NewGuid(), x.InstrumentId, x.Slug, x.Name, x.Type))
            .ToArray();

        try
        {
            foreach (var seed in currencySeeds)
            {
                foreach (var model in AllReportModels)
                {
                    SeedRawReportFiles(storageRoot, seed, model, reportDates);
                }
            }

            var instrumentRepository = new FakeInstrumentRepository(instruments);
            var latestReportRepository = new CotLatestReportFileQueryRepository([storageRoot]);
            var historicalReportRepository = new CotHistoricalReportFileQueryRepository([storageRoot]);
            var latestHandler = new GetCotLatestReportHandler(
                instrumentRepository,
                latestReportRepository,
                new GetCotLatestReportValidator());
            var availableDatesHandler = new GetCotAvailableDatesHandler(
                instrumentRepository,
                historicalReportRepository,
                new GetCotAvailableDatesValidator());

            foreach (var seed in currencySeeds)
            {
                foreach (var model in AllReportModels)
                {
                    var latestResult = await latestHandler.Handle(
                        new GetCotLatestReportQuery(seed.Slug, model, null),
                        CancellationToken.None);
                    var datesResult = await availableDatesHandler.Handle(
                        new GetCotAvailableDatesQuery(seed.Slug, model),
                        CancellationToken.None);

                    Assert.True(latestResult.IsSuccess, $"{seed.Slug} {model}: {latestResult.Message}");
                    Assert.NotNull(latestResult.Data);
                    Assert.Equal(seed.Slug, latestResult.Data!.InstrumentSlug);
                    Assert.Equal(reportDates[^1].ToDateTime(TimeOnly.MinValue), latestResult.Data.ReportDate);
                    Assert.NotEmpty(latestResult.Data.Categories);
                    Assert.True(latestResult.Data.OpenInterestAll > 0);

                    Assert.True(datesResult.IsSuccess, $"{seed.Slug} {model}: {datesResult.Message}");
                    Assert.NotNull(datesResult.Data);
                    Assert.Equal(reportDates, datesResult.Data!.AvailableDates);
                    Assert.Equal(reportDates[^1], datesResult.Data.LatestDate);
                }
            }
        }
        finally
        {
            if (Directory.Exists(storageRoot))
            {
                Directory.Delete(storageRoot, recursive: true);
            }
        }
    }

    private static CotInstrument CreateInstrument()
    {
        return new CotInstrument(
            Guid.Parse("5f8ce2f0-fcf5-4c4f-8d62-2a9627b6a37f"),
            "aud-cme-futures",
            "australian-dollar",
            "AUSTRALIAN DOLLAR",
            CotInstrumentType.Currencies);
    }

    private static CotInstrument CreateGoldInstrument()
    {
        return new CotInstrument(
            Guid.Parse("2c9d31c9-e6f2-46ef-a688-b97f7c7099ad"),
            "gold-comex-futures",
            "gold",
            "GOLD",
            CotInstrumentType.Metals);
    }

    private static CotLatestReportRawRecord CreateRawRecord(DateOnly reportDate, IReadOnlyDictionary<string, string?> fields)
    {
        return new CotLatestReportRawRecord(
            "AUSTRALIAN DOLLAR - CME",
            reportDate.ToDateTime(TimeOnly.MinValue),
            "(CONTRACTS OF AUD 100,000)",
            fields);
    }

    private static IReadOnlyDictionary<string, string?> CreateDisaggregatedFields(
        int managedMoneyLong = 310,
        int managedMoneyShort = 140)
    {
        return new Dictionary<string, string?>
        {
            ["market_and_exchange_names"] = "GOLD - COMMODITY EXCHANGE INC.",
            ["contract_units"] = "(100 TROY OUNCES)",
            ["open_interest_all"] = "5000",
            ["change_in_open_interest_all"] = "125",
            ["traders_tot_all"] = "120",
            ["prod_merc_positions_long"] = "800",
            ["prod_merc_positions_short"] = "1300",
            ["pct_of_oi_prod_merc_long"] = "16.0",
            ["pct_of_oi_prod_merc_short"] = "26.0",
            ["traders_prod_merc_long_all"] = "25",
            ["traders_prod_merc_short_all"] = "35",
            ["change_in_prod_merc_long"] = "20",
            ["change_in_prod_merc_short"] = "-10",
            ["swap_positions_long_all"] = "900",
            ["swap__positions_short_all"] = "700",
            ["swap__positions_spread_all"] = "50",
            ["pct_of_oi_swap_long_all"] = "18.0",
            ["pct_of_oi_swap_short_all"] = "14.0",
            ["pct_of_oi_swap_spread_all"] = "1.0",
            ["traders_swap_long_all"] = "20",
            ["traders_swap_short_all"] = "18",
            ["traders_swap_spread_all"] = "4",
            ["change_in_swap_long_all"] = "15",
            ["change_in_swap_short_all"] = "-7",
            ["change_in_swap_spread_all"] = "2",
            ["m_money_positions_long_all"] = managedMoneyLong.ToString(),
            ["m_money_positions_short_all"] = managedMoneyShort.ToString(),
            ["m_money_positions_spread"] = "80",
            ["pct_of_oi_m_money_long_all"] = "6.2",
            ["pct_of_oi_m_money_short_all"] = "2.8",
            ["pct_of_oi_m_money_spread"] = "1.6",
            ["traders_m_money_long_all"] = "30",
            ["traders_m_money_short_all"] = "15",
            ["traders_m_money_spread_all"] = "6",
            ["change_in_m_money_long_all"] = "40",
            ["change_in_m_money_short_all"] = "-30",
            ["change_in_m_money_spread"] = "5",
            ["other_rept_positions_long"] = "600",
            ["other_rept_positions_short"] = "500",
            ["other_rept_positions_spread"] = "60",
            ["pct_of_oi_other_rept_long"] = "12.0",
            ["pct_of_oi_other_rept_short"] = "10.0",
            ["pct_of_oi_other_rept_spread"] = "1.2",
            ["traders_other_rept_long_all"] = "12",
            ["traders_other_rept_short"] = "10",
            ["traders_other_rept_spread"] = "3",
            ["change_in_other_rept_long"] = "12",
            ["change_in_other_rept_short"] = "-8",
            ["change_in_other_rept_spread"] = "1",
            ["nonrept_positions_long_all"] = "400",
            ["nonrept_positions_short_all"] = "450",
            ["pct_of_oi_nonrept_long_all"] = "8.0",
            ["pct_of_oi_nonrept_short_all"] = "9.0",
            ["change_in_nonrept_long_all"] = "5",
            ["change_in_nonrept_short_all"] = "-3"
        };
    }

    private static void SeedRawReportFiles(
        string storageRoot,
        MarketDirectorySeedItem seed,
        CotReportModel model,
        IReadOnlyList<DateOnly> reportDates)
    {
        var modelFolder = model == CotReportModel.CotLegacy ? "legacy" : "normal";
        var instrumentDirectory = Path.Combine(storageRoot, modelFolder, seed.Slug);
        Directory.CreateDirectory(instrumentDirectory);

        foreach (var reportDate in reportDates)
        {
            var payload = BuildRawReportPayload(seed, reportDate);
            File.WriteAllText(Path.Combine(instrumentDirectory, $"{reportDate:yyyy-MM-dd}.json"), payload);

            if (reportDate == reportDates[^1])
            {
                File.WriteAllText(Path.Combine(instrumentDirectory, "latest.json"), payload);
            }
        }
    }

    private static string BuildRawReportPayload(MarketDirectorySeedItem seed, DateOnly reportDate)
    {
        var fields = new Dictionary<string, string?>
        {
            ["market_and_exchange_names"] = $"{seed.Name} - {seed.SourceExchangeName}",
            ["report_date_as_yyyy_mm_dd"] = $"{reportDate:yyyy-MM-dd}T00:00:00.000",
            ["contract_units"] = "(TEST CONTRACTS)",
            ["open_interest_all"] = "1000",
            ["change_in_open_interest_all"] = "25",
            ["traders_tot_all"] = "50",
            ["dealer_positions_long_all"] = "200",
            ["dealer_positions_short_all"] = "120",
            ["dealer_positions_spread_all"] = "15",
            ["pct_of_oi_dealer_long_all"] = "20.0",
            ["pct_of_oi_dealer_short_all"] = "12.0",
            ["pct_of_oi_dealer_spread_all"] = "1.5",
            ["traders_dealer_long_all"] = "5",
            ["traders_dealer_short_all"] = "4",
            ["traders_dealer_spread_all"] = "2",
            ["change_in_dealer_long_all"] = "10",
            ["change_in_dealer_short_all"] = "-5",
            ["change_in_dealer_spread_all"] = "1",
            ["asset_mgr_positions_long"] = "250",
            ["asset_mgr_positions_short"] = "100",
            ["asset_mgr_positions_spread"] = "20",
            ["pct_of_oi_asset_mgr_long"] = "25.0",
            ["pct_of_oi_asset_mgr_short"] = "10.0",
            ["pct_of_oi_asset_mgr_spread"] = "2.0",
            ["traders_asset_mgr_long_all"] = "7",
            ["traders_asset_mgr_short_all"] = "3",
            ["traders_asset_mgr_spread"] = "2",
            ["change_in_asset_mgr_long"] = "8",
            ["change_in_asset_mgr_short"] = "-2",
            ["change_in_asset_mgr_spread"] = "1",
            ["lev_money_positions_long"] = "300",
            ["lev_money_positions_short"] = "180",
            ["lev_money_positions_spread"] = "30",
            ["pct_of_oi_lev_money_long"] = "30.0",
            ["pct_of_oi_lev_money_short"] = "18.0",
            ["pct_of_oi_lev_money_spread"] = "3.0",
            ["traders_lev_money_long_all"] = "8",
            ["traders_lev_money_short_all"] = "6",
            ["traders_lev_money_spread"] = "2",
            ["change_in_lev_money_long"] = "12",
            ["change_in_lev_money_short"] = "-4",
            ["change_in_lev_money_spread"] = "2",
            ["other_rept_positions_long"] = "90",
            ["other_rept_positions_short"] = "70",
            ["other_rept_positions_spread"] = "5",
            ["pct_of_oi_other_rept_long"] = "9.0",
            ["pct_of_oi_other_rept_short"] = "7.0",
            ["pct_of_oi_other_rept_spread"] = "0.5",
            ["traders_other_rept_long_all"] = "3",
            ["traders_other_rept_short"] = "2",
            ["traders_other_rept_spread"] = "1",
            ["change_in_other_rept_long"] = "3",
            ["change_in_other_rept_short"] = "-1",
            ["change_in_other_rept_spread"] = "0",
            ["noncomm_positions_long_all"] = "350",
            ["noncomm_positions_short_all"] = "220",
            ["noncomm_postions_spread_all"] = "40",
            ["pct_of_oi_noncomm_long_all"] = "35.0",
            ["pct_of_oi_noncomm_short_all"] = "22.0",
            ["pct_of_oi_noncomm_spread"] = "4.0",
            ["traders_noncomm_long_all"] = "12",
            ["traders_noncomm_short_all"] = "9",
            ["traders_noncomm_spread_all"] = "4",
            ["change_in_noncomm_long_all"] = "15",
            ["change_in_noncomm_short_all"] = "-6",
            ["change_in_noncomm_spead_all"] = "2",
            ["comm_positions_long_all"] = "400",
            ["comm_positions_short_all"] = "500",
            ["pct_of_oi_comm_long_all"] = "40.0",
            ["pct_of_oi_comm_short_all"] = "50.0",
            ["traders_comm_long_all"] = "14",
            ["traders_comm_short_all"] = "16",
            ["change_in_comm_long_all"] = "20",
            ["change_in_comm_short_all"] = "-10",
            ["tot_rept_positions_long_all"] = "790",
            ["tot_rept_positions_short"] = "790",
            ["pct_of_oi_tot_rept_long_all"] = "79.0",
            ["pct_of_oi_tot_rept_short"] = "79.0",
            ["traders_tot_rept_long_all"] = "30",
            ["traders_tot_rept_short_all"] = "31",
            ["change_in_tot_rept_long_all"] = "35",
            ["change_in_tot_rept_short"] = "-16",
            ["nonrept_positions_long_all"] = "210",
            ["nonrept_positions_short_all"] = "210",
            ["pct_of_oi_nonrept_long_all"] = "21.0",
            ["pct_of_oi_nonrept_short_all"] = "21.0",
            ["change_in_nonrept_long_all"] = "5",
            ["change_in_nonrept_short_all"] = "-5"
        };

        return System.Text.Json.JsonSerializer.Serialize(new[] { fields });
    }

    private sealed class FakeInstrumentRepository : ICotInstrumentQueryRepository
    {
        private readonly IReadOnlyList<CotInstrument> _instruments;

        public FakeInstrumentRepository(CotInstrument? instrument)
            : this(instrument is null ? [] : [instrument])
        {
        }

        public FakeInstrumentRepository(IReadOnlyList<CotInstrument> instruments)
        {
            _instruments = instruments;
        }

        public CotInstrument Get(Guid id)
        {
            return _instruments.Single(x => x.Id == id);
        }

        public IList<CotInstrument> GetList()
        {
            return _instruments.ToList();
        }

        public bool Exists(Expression<Func<CotInstrument, bool>> expression)
        {
            return _instruments.Any(expression.Compile());
        }

        public Task<CotInstrument?> GetBySlugAsync(string instrumentSlug, CancellationToken cancellationToken)
        {
            var match = _instruments.FirstOrDefault(x =>
                string.Equals(x.InstrumentSlug, instrumentSlug, StringComparison.OrdinalIgnoreCase));

            return Task.FromResult(match);
        }

        public Task<IReadOnlyList<CotInstrument>> GetByGroupAsync(CotInstrumentType group, CancellationToken cancellationToken)
        {
            IReadOnlyList<CotInstrument> result = _instruments.Where(x => x.Type == group).ToArray();
            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<CotInstrument>> GetAllOrderedAsync(CancellationToken cancellationToken)
        {
            IReadOnlyList<CotInstrument> result = _instruments.OrderBy(x => x.InstrumentName).ToArray();
            return Task.FromResult(result);
        }
    }

    private sealed class FakeLatestReportRepository : ICotLatestReportQueryRepository
    {
        private readonly CotLatestReportRawRecord? _record;

        public FakeLatestReportRepository(CotLatestReportRawRecord? record)
        {
            _record = record;
        }

        public Task<CotLatestReportRawRecord?> GetAsync(
            string instrumentSlug,
            CotReportModel model,
            DateOnly? reportDate,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_record);
        }
    }

    private sealed class FakeHistoricalReportRepository : ICotHistoricalReportQueryRepository
    {
        private readonly IReadOnlyList<DateOnly> _availableDates;
        private readonly IReadOnlyList<CotLatestReportRawRecord> _records;

        public FakeHistoricalReportRepository(
            IReadOnlyList<DateOnly> availableDates,
            IReadOnlyList<CotLatestReportRawRecord> records)
        {
            _availableDates = availableDates;
            _records = records;
        }

        public Task<IReadOnlyList<DateOnly>> GetAvailableDatesAsync(
            string instrumentSlug,
            CotReportModel model,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_availableDates);
        }

        public Task<IReadOnlyList<CotLatestReportRawRecord>> GetRangeAsync(
            string instrumentSlug,
            CotReportModel model,
            DateOnly fromDate,
            DateOnly toDate,
            CancellationToken cancellationToken)
        {
            var result = _records
                .Where(x => x.ReportDate is not null)
                .Where(x =>
                {
                    var date = DateOnly.FromDateTime(x.ReportDate!.Value);
                    return date >= fromDate && date <= toDate;
                })
                .OrderBy(x => x.ReportDate)
                .ToArray();

            return Task.FromResult<IReadOnlyList<CotLatestReportRawRecord>>(result);
        }
    }
}
