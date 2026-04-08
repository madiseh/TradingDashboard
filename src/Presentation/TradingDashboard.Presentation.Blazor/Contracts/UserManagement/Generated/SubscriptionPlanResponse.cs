using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.UserManagement.Generated;

public sealed class SubscriptionPlanResponse
{
    [JsonPropertyName("monthlyPriceUsd")]
    public double MonthlyPriceUsd { get; set; }

    [JsonPropertyName("planCode")]
    public string? PlanCode { get; set; }

    [JsonPropertyName("planName")]
    public string? PlanName { get; set; }

    [JsonPropertyName("yearlyDiscountPercentage")]
    public double YearlyDiscountPercentage { get; set; }

    [JsonPropertyName("yearlyPriceUsd")]
    public double YearlyPriceUsd { get; set; }
}
