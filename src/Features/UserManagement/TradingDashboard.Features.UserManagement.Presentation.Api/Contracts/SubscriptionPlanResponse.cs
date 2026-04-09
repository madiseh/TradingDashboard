namespace TradingDashboard.Presentation.Api.UserManagement.Contracts;

public sealed record SubscriptionPlanResponse(
    string PlanCode,
    string PlanName,
    decimal MonthlyPriceUsd,
    decimal YearlyPriceUsd,
    decimal YearlyDiscountPercentage);
