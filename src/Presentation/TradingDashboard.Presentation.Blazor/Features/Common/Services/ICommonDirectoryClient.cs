namespace TradingDashboard.Presentation.Blazor.Features.Common.Services;

/// <summary>
/// Provides read-only access to common market-directory endpoints for presentation flows.
/// </summary>
public interface ICommonDirectoryClient
{
    /// <summary>
    /// Fetches grouped market directory data used by the Home page.
    /// </summary>
    Task<MarketsDirectoryResponse?> GetMarketsDirectoryAsync(CancellationToken cancellationToken = default);
}
