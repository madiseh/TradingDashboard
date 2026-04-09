using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.UserManagement.Generated;

public sealed class SubscriptionResponse
{
    [JsonPropertyName("basePriceUsd")]
    public double BasePriceUsd { get; set; }

    [JsonPropertyName("billingCycle")]
    public string? BillingCycle { get; set; }

    [JsonPropertyName("discountPercentage")]
    public double DiscountPercentage { get; set; }

    [JsonPropertyName("expiresAtUtc")]
    public DateTime ExpiresAtUtc { get; set; }

    [JsonPropertyName("finalPriceUsd")]
    public double FinalPriceUsd { get; set; }

    [JsonPropertyName("planCode")]
    public string? PlanCode { get; set; }

    [JsonPropertyName("planName")]
    public string? PlanName { get; set; }

    [JsonPropertyName("startsAtUtc")]
    public DateTime StartsAtUtc { get; set; }

    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("subscriptionId")]
    public string? SubscriptionId { get; set; }

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
}
