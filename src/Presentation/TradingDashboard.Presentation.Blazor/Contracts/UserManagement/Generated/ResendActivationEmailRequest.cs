using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.UserManagement.Generated;

public sealed class ResendActivationEmailRequest
{
    [JsonPropertyName("identity")]
    public string? Identity { get; set; }
}
