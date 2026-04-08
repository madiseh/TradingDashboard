namespace TradingDashboard.Presentation.Blazor.Components.Pages.Pricing;

public partial class Pricing
{
    private enum PricingBillingCycle
    {
        Monthly = 1,
        Yearly = 2
    }

    private sealed record PricingPlanApiModel(
        string PlanCode,
        string PlanName,
        decimal MonthlyPriceUsd,
        decimal YearlyPriceUsd,
        decimal YearlyDiscountPercentage);

    private sealed record PricingSubscriptionApiModel(
        Guid SubscriptionId,
        Guid UserId,
        string PlanCode,
        string PlanName,
        string BillingCycle,
        string Status,
        decimal BasePriceUsd,
        decimal DiscountPercentage,
        decimal FinalPriceUsd,
        DateTime StartsAtUtc,
        DateTime ExpiresAtUtc);
}
