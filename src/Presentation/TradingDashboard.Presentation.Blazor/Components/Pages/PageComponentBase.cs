namespace TradingDashboard.Presentation.Blazor.Components.Pages;

public abstract class PageComponentBase : ComponentBase
{
    [Inject]
    protected IHttpClientFactory HttpClientFactory { get; set; } = default!;
}
