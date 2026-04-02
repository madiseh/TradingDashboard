namespace TradingDashboard.Presentation.MauiHybrid.Services;

public sealed class ApiHttpClientFactory
{
    private readonly IConfiguration _configuration;

    public ApiHttpClientFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public HttpClient CreateClient(string clientName)
    {
        return new HttpClient
        {
            BaseAddress = CreateEndpointUri(clientName)
        };
    }

    private Uri CreateEndpointUri(string clientName)
    {
        var fallbackUrl = clientName switch
        {
            HttpClientNames.ApiCot => "https://localhost:7116",
            HttpClientNames.ApiLogin => "https://localhost:7181",
            _ => throw new InvalidOperationException($"Unsupported API client name '{clientName}'.")
        };

        var platformSpecificKey = DeviceInfo.Platform == DevicePlatform.Android
            ? $"{clientName}:AndroidEmulatorBaseUrl"
            : $"{clientName}:BaseUrl";

        var configuredUrl = _configuration[platformSpecificKey]
            ?? _configuration[$"{clientName}:BaseUrl"]
            ?? fallbackUrl;

        return new Uri(configuredUrl, UriKind.Absolute);
    }
}
