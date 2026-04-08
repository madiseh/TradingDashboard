using System.Text;
using System.Xml.Linq;

namespace TradingDashboard.Presentation.Blazor.Services.SiteIndexing;

public static class SiteIndexingEndpointRouteBuilderExtensions
{
    public static IEndpointRouteBuilder MapSiteIndexingEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/robots.txt", () =>
        {
            var lines = new[]
            {
                "User-agent: *",
                "Allow: /",
                "Disallow: /login",
                "Disallow: /Error",
                $"Sitemap: {SiteBranding.AbsoluteUrl("/sitemap.xml")}"
            };

            return Results.Text(string.Join(Environment.NewLine, lines), "text/plain; charset=utf-8", Encoding.UTF8);
        });

        endpoints.MapGet("/sitemap.xml", async (ISiteIndexingService siteIndexingService, CancellationToken cancellationToken) =>
        {
            var sitemap = await siteIndexingService.GetSitemapEntriesAsync(cancellationToken);
            var document = BuildSitemapDocument(sitemap);
            return Results.Text(document, "application/xml; charset=utf-8", Encoding.UTF8);
        });

        return endpoints;
    }

    private static string BuildSitemapDocument(IReadOnlyList<SitemapUrlEntry> entries)
    {
        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement(
                ns + "urlset",
                entries.Select(entry =>
                {
                    var urlElement = new XElement(ns + "url", new XElement(ns + "loc", entry.Location));

                    if (entry.LastModifiedUtc is not null)
                    {
                        urlElement.Add(new XElement(ns + "lastmod", entry.LastModifiedUtc.Value.ToString("yyyy-MM-dd")));
                    }

                    return urlElement;
                })));

        return document.ToString(SaveOptions.DisableFormatting);
    }
}
