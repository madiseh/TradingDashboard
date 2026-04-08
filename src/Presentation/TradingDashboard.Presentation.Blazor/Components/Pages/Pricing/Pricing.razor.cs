using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Localization;
using MudBlazor;
using TradingDashboard.Presentation.Blazor.Services;
using TradingDashboard.Presentation.Common.Resources.Components.Pages.Pricing.Pricing;
using UserManagementApiErrorResponse = TradingDashboard.Presentation.Blazor.Contracts.UserManagement.Generated.ApiErrorResponse;

namespace TradingDashboard.Presentation.Blazor.Components.Pages.Pricing;

public partial class Pricing
{
    private static readonly IReadOnlyList<string> FeatureKeys =
    [
        "Features.Item1",
        "Features.Item2",
        "Features.Item3",
        "Features.Item4",
        "Features.Item5",
        "Features.Item6",
        "Features.Item7",
        "Features.Item8"
    ];

    private bool _hasLoaded;
    private bool _isLoading = true;
    private bool _isSubmitting;
    private string? _errorMessage;
    private PricingPlanApiModel? _plan;
    private PricingSubscriptionApiModel? _subscription;
    private PricingBillingCycle _selectedBillingCycle = PricingBillingCycle.Monthly;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private AuthSession AuthSession { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private IStringLocalizer<PricingResource> L { get; set; } = default!;

    [SupplyParameterFromQuery(Name = "billingCycle")]
    public string? BillingCycle { get; set; }

    private bool IsYearlySelected => _selectedBillingCycle == PricingBillingCycle.Yearly;

    protected override async Task OnParametersSetAsync()
    {
        _selectedBillingCycle = ParseBillingCycle(BillingCycle);

        if (_hasLoaded)
        {
            return;
        }

        _hasLoaded = true;
        await LoadAsync();
    }

    private async Task LoadAsync()
    {
        _isLoading = true;
        _errorMessage = null;

        try
        {
            var client = HttpClientFactory.CreateClient(HttpClientNames.ApiLogin);
            var plansResponse = await client.GetAsync(RoutePaths.Api.Login.SubscriptionPlans);
            if (!plansResponse.IsSuccessStatusCode)
            {
                _errorMessage = await ExtractErrorAsync(plansResponse, L["Errors.LoadPlans"]);
                _plan = null;
                _subscription = null;
                return;
            }

            var plans = await plansResponse.Content.ReadFromJsonAsync<IReadOnlyList<PricingPlanApiModel>>();
            _plan = plans?.FirstOrDefault();

            if (_plan is null)
            {
                _subscription = null;
                return;
            }

            await LoadCurrentSubscriptionAsync();
        }
        catch (Exception ex)
        {
            _errorMessage = L["Errors.LoadPlansWithMessage", ex.Message];
            _plan = null;
            _subscription = null;
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task LoadCurrentSubscriptionAsync()
    {
        if (!AuthSession.IsAuthenticated || string.IsNullOrWhiteSpace(AuthSession.AccessToken))
        {
            _subscription = null;
            return;
        }

        var client = HttpClientFactory.CreateClient(HttpClientNames.ApiLogin);
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthSession.AccessToken);

        var response = await client.GetAsync(RoutePaths.Api.Login.MySubscription);
        if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
        {
            _subscription = null;
            return;
        }

        if (!response.IsSuccessStatusCode)
        {
            _subscription = null;
            return;
        }

        _subscription = await response.Content.ReadFromJsonAsync<PricingSubscriptionApiModel>();
    }

    private async Task HandlePrimaryActionAsync()
    {
        if (_plan is null)
        {
            return;
        }

        if (!AuthSession.IsAuthenticated || string.IsNullOrWhiteSpace(AuthSession.AccessToken))
        {
            NavigationManager.NavigateTo(BuildLoginRedirectUrl());
            return;
        }

        _isSubmitting = true;
        _errorMessage = null;

        try
        {
            var client = HttpClientFactory.CreateClient(HttpClientNames.ApiLogin);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthSession.AccessToken);

            var response = await client.PostAsJsonAsync(
                RoutePaths.Api.Login.SubscriptionPurchase,
                new
                {
                    PlanCode = _plan.PlanCode,
                    BillingCycle = GetSelectedBillingCycleValue()
                });

            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = await ExtractErrorAsync(response, L["Errors.Purchase"]);
                Snackbar.Add(_errorMessage, Severity.Error);
                return;
            }

            _subscription = await response.Content.ReadFromJsonAsync<PricingSubscriptionApiModel>();
            Snackbar.Add(
                IsYearlySelected ? L["Success.PurchaseYearly"] : L["Success.PurchaseMonthly"],
                Severity.Success);
        }
        catch (Exception ex)
        {
            _errorMessage = L["Errors.PurchaseWithMessage", ex.Message];
            Snackbar.Add(_errorMessage, Severity.Error);
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private void SelectCycle(PricingBillingCycle cycle)
    {
        _selectedBillingCycle = cycle;
    }

    private string GetCycleButtonClass(PricingBillingCycle cycle)
    {
        return cycle == _selectedBillingCycle
            ? "pricing-page__cycle-button pricing-page__cycle-button--active"
            : "pricing-page__cycle-button";
    }

    private decimal GetSelectedPrice()
    {
        if (_plan is null)
        {
            return 0m;
        }

        return IsYearlySelected ? _plan.YearlyPriceUsd : _plan.MonthlyPriceUsd;
    }

    private string GetSelectedPriceUnitText()
    {
        return IsYearlySelected ? L["Labels.PerYear"] : L["Labels.PerMonth"];
    }

    private string GetSelectedBillingLabel()
    {
        return IsYearlySelected ? L["Labels.BilledYearly"] : L["Labels.BilledMonthly"];
    }

    private string GetLocalizedCycleLabel(string cycle)
    {
        return string.Equals(cycle, "yearly", StringComparison.OrdinalIgnoreCase)
            ? L["Labels.Yearly"]
            : L["Labels.Monthly"];
    }

    private string GetPrimaryActionText()
    {
        if (_isSubmitting)
        {
            return L["State.Processing"];
        }

        if (!AuthSession.IsAuthenticated)
        {
            return L["Actions.SignInToSubscribe"];
        }

        return IsYearlySelected ? L["Actions.BuyYearly"] : L["Actions.BuyMonthly"];
    }

    private string BuildLoginRedirectUrl()
    {
        var pricingReturnUrl = QueryHelpers.AddQueryString(
            RoutePaths.Ui.Pricing,
            "billingCycle",
            GetSelectedBillingCycleValue());

        return QueryHelpers.AddQueryString(
            RoutePaths.Ui.Login,
            "returnUrl",
            pricingReturnUrl);
    }

    private string GetSelectedBillingCycleValue()
    {
        return IsYearlySelected ? "yearly" : "monthly";
    }

    private static PricingBillingCycle ParseBillingCycle(string? billingCycle)
    {
        return string.Equals(billingCycle, "yearly", StringComparison.OrdinalIgnoreCase)
            ? PricingBillingCycle.Yearly
            : PricingBillingCycle.Monthly;
    }

    private static string FormatPercentage(decimal value)
    {
        return $"{value:0.##}%";
    }

    private static string FormatUsd(decimal value)
    {
        return value.ToString("C0", CultureInfo.GetCultureInfo("en-US"));
    }

    private static string FormatDate(DateTime utcDate)
    {
        return utcDate.ToLocalTime().ToString("yyyy-MM-dd");
    }

    private static async Task<string> ExtractErrorAsync(HttpResponseMessage response, string fallback)
    {
        var payload = await response.Content.ReadFromJsonAsync<UserManagementApiErrorResponse>();
        if (payload is null)
        {
            return fallback;
        }

        return payload.Errors is { Count: > 0 }
            ? string.Join(" | ", payload.Errors)
            : payload.Message ?? fallback;
    }
}
