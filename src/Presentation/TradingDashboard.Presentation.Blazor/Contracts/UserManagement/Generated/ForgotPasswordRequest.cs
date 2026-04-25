using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.UserManagement.Generated;

public sealed class ForgotPasswordRequest
{
    [JsonPropertyName("identity")]
    public string? Identity { get; set; }
}
