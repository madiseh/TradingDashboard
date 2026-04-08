using Microsoft.Extensions.Caching.Memory;

namespace TradingDashboard.Presentation.Blazor.Services.SiteIndexing;

public sealed class SiteIndexingService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache) : ISiteIndexingService
{
    private static readonly TimeSpan SitemapCacheDuration = TimeSpan.FromMinutes(15);
    private const string SitemapCacheKey = "site-indexing:sitemap-entries";

    private static readonly string[] StaticRoutes =
    [
        RoutePaths.Ui.Home,
        RoutePaths.Ui.CotHome,
        RoutePaths.Ui.CurrencyStrength,
        RoutePaths.Ui.Releases
    ];

    public async Task<IReadOnlyList<SitemapUrlEntry>> GetSitemapEntriesAsync(CancellationToken cancellationToken = default)
    {
        return await memoryCache.GetOrCreateAsync(SitemapCacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = SitemapCacheDuration;

            var entries = new Dictionary<string, SitemapUrlEntry>(StringComparer.OrdinalIgnoreCase);

            foreach (var route in StaticRoutes)
            {
                AddEntry(entries, route);
            }

            await AddCotEntriesAsync(entries, cancellationToken);
            await AddCurrencyStrengthEntriesAsync(entries, cancellationToken);

            return (IReadOnlyList<SitemapUrlEntry>)entries.Values
                .OrderBy(item => item.Location, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }) ?? [];
    }

    private async Task AddCotEntriesAsync(
        IDictionary<string, SitemapUrlEntry> entries,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HttpClientNames.ApiCot);
            using var response = await client.GetAsync(RoutePaths.Api.Cot.CotInstrumentsByGroup, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return;
            }

            var payload = await response.Content.ReadFromJsonAsync<IReadOnlyList<GetCotInstrumentsByGroupResponse>>(cancellationToken);
            if (payload is null)
            {
                return;
            }

            var slugs = payload
                .SelectMany(group => group.Items ?? [])
                .Select(item => item.InstrumentSlug)
                .Where(slug => !string.IsNullOrWhiteSpace(slug))
                .Distinct(StringComparer.OrdinalIgnoreCase);

            foreach (var slug in slugs)
            {
                var escapedSlug = Uri.EscapeDataString(slug!);
                AddEntry(entries, $"{RoutePaths.Ui.CotReports}/{escapedSlug}?model={CotReportModel.Cot.ToContractValue()}");
                AddEntry(entries, $"{RoutePaths.Ui.CotReports}/{escapedSlug}?model={CotReportModel.CotLegacy.ToContractValue()}");
            }
        }
        catch
        {
        }
    }

    private async Task AddCurrencyStrengthEntriesAsync(
        IDictionary<string, SitemapUrlEntry> entries,
        CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(HttpClientNames.ApiCurrencyStrength);
            var payload = await client.GetFromJsonAsync<CurrencyStrengthTimeframesResponse>(RoutePaths.Api.CurrencyStrength.Timeframes, cancellationToken);
            if (payload?.Items is null || payload.Items.Count == 0)
            {
                return;
            }

            var orderedTimeframes = payload.Items
                .Where(item => !string.IsNullOrWhiteSpace(item.Key))
                .OrderBy(item => item.SortOrder)
                .ToList();

            var defaultTimeframeKey = orderedTimeframes.FirstOrDefault(item => item.IsDefault)?.Key ?? orderedTimeframes[0].Key;

            foreach (var timeframe in orderedTimeframes)
            {
                if (string.Equals(timeframe.Key, defaultTimeframeKey, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                AddEntry(entries, $"{RoutePaths.Ui.CurrencyStrength}?timeframe={Uri.EscapeDataString(timeframe.Key)}");
            }
        }
        catch
        {
        }
    }

    private static void AddEntry(IDictionary<string, SitemapUrlEntry> entries, string relativeOrAbsoluteUrl)
    {
        var absoluteUrl = SiteBranding.AbsoluteUrl(relativeOrAbsoluteUrl);
        entries[absoluteUrl] = new SitemapUrlEntry(absoluteUrl);
    }
}
