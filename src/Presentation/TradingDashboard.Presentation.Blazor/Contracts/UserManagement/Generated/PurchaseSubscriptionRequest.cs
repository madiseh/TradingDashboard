using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.UserManagement.Generated;

public sealed class PurchaseSubscriptionRequest
{
    [JsonPropertyName("billingCycle")]
    public string? BillingCycle { get; set; }

    [JsonPropertyName("planCode")]
    public string? PlanCode { get; set; }
}
