using TradingDashboard.Presentation.Common.Resources.Components.Pages.MauiHybrid.Home;

namespace TradingDashboard.Presentation.MauiHybrid.Components.Pages;

public partial class Home
{
    [Inject]
    private IConfiguration Configuration { get; set; } = default!;

    [Inject]
    private IStringLocalizer<MauiHybridHomeResource> Localizer { get; set; } = default!;

    protected IReadOnlyList<HomeStatusCardViewModel> Cards { get; private set; } = Array.Empty<HomeStatusCardViewModel>();
    protected string CotApiBaseUrl { get; private set; } = string.Empty;
    protected string LoginApiBaseUrl { get; private set; } = string.Empty;

    protected override void OnInitialized()
    {
        CotApiBaseUrl = ResolveApiBaseUrl(HttpClientNames.ApiCot, "https://localhost:7116");
        LoginApiBaseUrl = ResolveApiBaseUrl(HttpClientNames.ApiLogin, "https://localhost:7181");

        Cards =
        [
            new HomeStatusCardViewModel(
                Localizer["Cards.Platforms.Title"],
                Localizer["Cards.Platforms.Value"],
                Localizer["Cards.Platforms.Description"]),
            new HomeStatusCardViewModel(
                Localizer["Cards.Architecture.Title"],
                Localizer["Cards.Architecture.Value"],
                Localizer["Cards.Architecture.Description"]),
            new HomeStatusCardViewModel(
                Localizer["Cards.Configuration.Title"],
                DeviceInfo.Platform == DevicePlatform.Android
                    ? Localizer["Cards.Configuration.AndroidValue"]
                    : Localizer["Cards.Configuration.DefaultValue"],
                Localizer["Cards.Configuration.Description"])
        ];
    }

    private string ResolveApiBaseUrl(string clientName, string fallbackUrl)
    {
        var platformSpecificKey = DeviceInfo.Platform == DevicePlatform.Android
            ? $"{clientName}:AndroidEmulatorBaseUrl"
            : $"{clientName}:BaseUrl";

        return Configuration[platformSpecificKey]
            ?? Configuration[$"{clientName}:BaseUrl"]
            ?? fallbackUrl;
    }
}
