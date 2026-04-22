using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.UserManagement.Generated;

public sealed class AuthActionResponse
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
