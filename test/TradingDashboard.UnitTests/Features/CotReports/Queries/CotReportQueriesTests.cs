namespace TradingDashboard.UnitTests.Features.CotReports.Queries;

public sealed class CotReportQueriesTests
{
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

    private static CotInstrument CreateInstrument()
    {
        return new CotInstrument(
            Guid.Parse("5f8ce2f0-fcf5-4c4f-8d62-2a9627b6a37f"),
            "aud-cme-futures",
            "australian-dollar",
            "AUSTRALIAN DOLLAR",
            CotInstrumentType.Currencies);
    }

    private static CotLatestReportRawRecord CreateRawRecord(DateOnly reportDate, IReadOnlyDictionary<string, string?> fields)
    {
        return new CotLatestReportRawRecord(
            "AUSTRALIAN DOLLAR - CME",
            reportDate.ToDateTime(TimeOnly.MinValue),
            "(CONTRACTS OF AUD 100,000)",
            fields);
    }

    private sealed class FakeInstrumentRepository : ICotInstrumentQueryRepository
    {
        private readonly CotInstrument? _instrument;

        public FakeInstrumentRepository(CotInstrument? instrument)
        {
            _instrument = instrument;
        }

        public CotInstrument Get(Guid id)
        {
            return _instrument!;
        }

        public IList<CotInstrument> GetList()
        {
            return _instrument is null ? [] : [_instrument];
        }

        public bool Exists(Expression<Func<CotInstrument, bool>> expression)
        {
            return _instrument is not null && expression.Compile().Invoke(_instrument);
        }

        public Task<CotInstrument?> GetBySlugAsync(string instrumentSlug, CancellationToken cancellationToken)
        {
            var match = _instrument is not null &&
                        string.Equals(_instrument.InstrumentSlug, instrumentSlug, StringComparison.OrdinalIgnoreCase);

            return Task.FromResult(match ? _instrument : null);
        }

        public Task<IReadOnlyList<CotInstrument>> GetByGroupAsync(CotInstrumentType group, CancellationToken cancellationToken)
        {
            IReadOnlyList<CotInstrument> result = _instrument is not null && _instrument.Type == group ? [_instrument] : [];
            return Task.FromResult(result);
        }

        public Task<IReadOnlyList<CotInstrument>> GetAllOrderedAsync(CancellationToken cancellationToken)
        {
            IReadOnlyList<CotInstrument> result = _instrument is null ? [] : [_instrument];
            return Task.FromResult(result);
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
