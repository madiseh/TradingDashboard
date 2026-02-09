using System.Net.Http.Json;
using TradingDashboard.Presentation.Blazor.Features.Common.Contracts;
using TradingDashboard.Presentation.Common.Routing;

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
