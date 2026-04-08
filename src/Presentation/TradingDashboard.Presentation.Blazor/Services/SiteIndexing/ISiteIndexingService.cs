namespace TradingDashboard.Presentation.Blazor.Services.SiteIndexing;

public interface ISiteIndexingService
{
    Task<IReadOnlyList<SitemapUrlEntry>> GetSitemapEntriesAsync(CancellationToken cancellationToken = default);
}
