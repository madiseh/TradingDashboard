namespace TradingDashboard.Application.Features.Subscriptions;

public sealed record SubscriptionSummaryResult(
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
