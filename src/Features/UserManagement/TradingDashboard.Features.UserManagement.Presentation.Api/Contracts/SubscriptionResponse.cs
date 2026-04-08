namespace TradingDashboard.Presentation.Api.UserManagement.Contracts;

public sealed record SubscriptionResponse(
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
