namespace TradingDashboard.Application.Common.Subscriptions;

public static class SubscriptionPlanCatalog
{
    private static readonly IReadOnlyList<SubscriptionPlanDefinition> Plans =
    [
        new("premium", "TerminalPrime Premium", 149m, 1490m)
    ];

    public static IReadOnlyList<SubscriptionPlanDefinition> GetPlans()
    {
        return Plans;
    }

    public static bool TryGet(string? planCode, out SubscriptionPlanDefinition? plan)
    {
        plan = Plans.FirstOrDefault(x =>
            string.Equals(x.PlanCode, planCode?.Trim(), StringComparison.OrdinalIgnoreCase));

        return plan is not null;
    }
}

public sealed record SubscriptionPlanDefinition(
    string PlanCode,
    string PlanName,
    decimal MonthlyPriceUsd,
    decimal YearlyPriceUsd)
{
    public decimal YearlyBasePriceUsd => MonthlyPriceUsd * 12m;

    public decimal YearlyDiscountPercentage =>
        YearlyBasePriceUsd <= 0
            ? 0
            : decimal.Round((1m - (YearlyPriceUsd / YearlyBasePriceUsd)) * 100m, 2, MidpointRounding.AwayFromZero);
}
