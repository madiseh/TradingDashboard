namespace TradingDashboard.Presentation.Blazor.Services.SiteIndexing;

public sealed record SitemapUrlEntry(string Location, DateTimeOffset? LastModifiedUtc = null);
