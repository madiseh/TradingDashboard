using TradingDashboard.Domain.Common.Common;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Domain.Entities;

public sealed class Subscription : BaseAuditableEntity<Guid>
{
    public Guid UserId { get; private set; }
    public string PlanCode { get; private set; }
    public string PlanName { get; private set; }
    public SubscriptionBillingCycle BillingCycle { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public decimal BasePriceUsd { get; private set; }
    public decimal DiscountPercentage { get; private set; }
    public decimal FinalPriceUsd { get; private set; }
    public DateTime StartsAtUtc { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }
    public DateTime PurchasedAtUtc { get; private set; }

    public Subscription(
        Guid id,
        Guid userId,
        string planCode,
        string planName,
        SubscriptionBillingCycle billingCycle,
        decimal basePriceUsd,
        decimal discountPercentage,
        decimal finalPriceUsd,
        DateTime purchasedAtUtc)
        : base(id, purchasedAtUtc)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        }

        UserId = userId;
        PlanCode = NormalizeRequired(planCode, nameof(planCode));
        PlanName = NormalizeRequired(planName, nameof(planName));
        BillingCycle = billingCycle;
        Status = SubscriptionStatus.Active;
        BasePriceUsd = NormalizeAmount(basePriceUsd, nameof(basePriceUsd));
        DiscountPercentage = NormalizeDiscount(discountPercentage);
        FinalPriceUsd = NormalizeAmount(finalPriceUsd, nameof(finalPriceUsd));
        StartsAtUtc = purchasedAtUtc;
        ExpiresAtUtc = CalculateExpiration(purchasedAtUtc, billingCycle);
        PurchasedAtUtc = purchasedAtUtc;
    }

    public void Purchase(
        string planCode,
        string planName,
        SubscriptionBillingCycle billingCycle,
        decimal basePriceUsd,
        decimal discountPercentage,
        decimal finalPriceUsd,
        DateTime purchasedAtUtc)
    {
        var effectiveStart = IsActiveAt(purchasedAtUtc) ? StartsAtUtc : purchasedAtUtc;
        var expirationSeed = IsActiveAt(purchasedAtUtc) ? ExpiresAtUtc : purchasedAtUtc;

        PlanCode = NormalizeRequired(planCode, nameof(planCode));
        PlanName = NormalizeRequired(planName, nameof(planName));
        BillingCycle = billingCycle;
        Status = SubscriptionStatus.Active;
        BasePriceUsd = NormalizeAmount(basePriceUsd, nameof(basePriceUsd));
        DiscountPercentage = NormalizeDiscount(discountPercentage);
        FinalPriceUsd = NormalizeAmount(finalPriceUsd, nameof(finalPriceUsd));
        StartsAtUtc = effectiveStart;
        ExpiresAtUtc = CalculateExpiration(expirationSeed, billingCycle);
        PurchasedAtUtc = purchasedAtUtc;

        MarkUpdated(purchasedAtUtc);
    }

    public SubscriptionStatus ResolveStatus(DateTime utcNow)
    {
        return IsActiveAt(utcNow)
            ? SubscriptionStatus.Active
            : SubscriptionStatus.Expired;
    }

    private bool IsActiveAt(DateTime utcNow)
    {
        return Status == SubscriptionStatus.Active && ExpiresAtUtc > utcNow;
    }

    private static DateTime CalculateExpiration(DateTime seedUtc, SubscriptionBillingCycle billingCycle)
    {
        return billingCycle == SubscriptionBillingCycle.Yearly
            ? seedUtc.AddYears(1)
            : seedUtc.AddMonths(1);
    }

    private static string NormalizeRequired(string value, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{argumentName} cannot be empty.", argumentName);
        }

        return value.Trim();
    }

    private static decimal NormalizeAmount(decimal value, string argumentName)
    {
        if (value <= 0)
        {
            throw new ArgumentOutOfRangeException(argumentName, "Amount must be greater than zero.");
        }

        return decimal.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    private static decimal NormalizeDiscount(decimal value)
    {
        if (value < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Discount cannot be negative.");
        }

        return decimal.Round(value, 2, MidpointRounding.AwayFromZero);
    }

    private Subscription() : base(Guid.Empty, DateTime.UtcNow)
    {
        UserId = Guid.Empty;
        PlanCode = string.Empty;
        PlanName = string.Empty;
        BillingCycle = SubscriptionBillingCycle.Monthly;
        Status = SubscriptionStatus.Active;
        BasePriceUsd = 0;
        DiscountPercentage = 0;
        FinalPriceUsd = 0;
        StartsAtUtc = DateTime.UtcNow;
        ExpiresAtUtc = DateTime.UtcNow;
        PurchasedAtUtc = DateTime.UtcNow;
    }
}
