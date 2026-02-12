namespace TradingDashboard.Presentation.Blazor.Features.Common.Services;

public sealed class CommonDirectoryClient(HttpClient httpClient) : ICommonDirectoryClient
{
    public async Task<MarketsDirectoryResponse?> GetMarketsDirectoryAsync(CancellationToken cancellationToken = default)
    {
        return await httpClient.GetFromJsonAsync<MarketsDirectoryResponse>(
            RoutePaths.Api.MarketsDirectory,
            cancellationToken);
    }
}
