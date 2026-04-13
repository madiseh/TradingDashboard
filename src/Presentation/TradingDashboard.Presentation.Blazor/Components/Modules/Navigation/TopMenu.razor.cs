using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using TradingDashboard.Presentation.Blazor.Services;
using TradingDashboard.Presentation.Common.Resources.Components.Modules.Navigation.TopMenu;

namespace TradingDashboard.Presentation.Blazor.Components.Modules.Navigation;

public partial class TopMenu
{
    private string _selectedCulture = "en";

    private static readonly IReadOnlyList<LanguageOption> SupportedLanguages =
    [
        new("en", "Language.Name.en", "Language.Short.en"),
        new("fa", "Language.Name.fa", "Language.Short.fa"),
        new("tr", "Language.Name.tr", "Language.Short.tr"),
        new("de", "Language.Name.de", "Language.Short.de")
    ];

    private IReadOnlyList<NavigationItem> NavigationItems =>
    [
        new("Menu.Home", RoutePaths.Ui.Home),
        new("Menu.Cot", RoutePaths.Ui.CotHome),
        new("Menu.CurrencyStrength", RoutePaths.Ui.CurrencyStrength)
    ];

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IJSRuntime JSRuntime { get; set; } = default!;

    [Inject]
    private IStringLocalizer<TopMenuResource> L { get; set; } = default!;

    [Inject]
    private PortalNavigationService PortalNavigation { get; set; } = default!;

    private IReadOnlyList<AuthActionItem> AuthActions =>
    [
        new(L["Actions.Login"], PortalNavigation.BuildLoginUrl(), "btn_theme td-auth-action td-auth-action--secondary"),
        new(L["Actions.SignUp"], PortalNavigation.BuildLoginUrl(mode: "signup"), "btn_theme btn_theme_active td-auth-action")
    ];

    protected override void OnInitialized()
    {
        _selectedCulture = NormalizeCulture(CultureInfo.CurrentUICulture.Name);
    }

    private string GetLanguageLabel(LanguageOption language)
    {
        return L[language.LabelKey];
    }

    private string GetShortLanguageLabel(LanguageOption language)
    {
        return L[language.ShortLabelKey];
    }

    private string GetNavigationLabel(NavigationItem item)
    {
        return L[item.LabelKey];
    }

    private async Task NavigateFromMobileMenuAsync(string href)
    {
        await JSRuntime.InvokeVoidAsync("tradingDashboardNavigation.closeOffcanvas", "offcanvasRight");
        await Task.Delay(200);
        NavigationManager.NavigateTo(href);
    }

    private async Task OnCultureChangedAsync(string culture)
    {
        var normalizedCulture = NormalizeCulture(culture);
        _selectedCulture = normalizedCulture;

        await JSRuntime.InvokeVoidAsync("tradingDashboardCulture.set", normalizedCulture);

        var currentUri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        NavigationManager.NavigateTo(currentUri.PathAndQuery, forceLoad: true);
    }

    private static string NormalizeCulture(string? culture)
    {
        if (SupportedLanguages.Any(language => string.Equals(language.Code, culture, StringComparison.OrdinalIgnoreCase)))
        {
            return culture!;
        }

        return culture?.Split('-', StringSplitOptions.RemoveEmptyEntries)[0].ToLowerInvariant() switch
        {
            "fa" => "fa",
            "tr" => "tr",
            "de" => "de",
            _ => "en"
        };
    }

    private sealed record LanguageOption(string Code, string LabelKey, string ShortLabelKey);

    private sealed record NavigationItem(string LabelKey, string Href);
}
