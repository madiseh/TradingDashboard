using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.UserManagement.Generated;

public sealed class ChangeUserPasswordRequest
{
    [JsonPropertyName("newPassword")]
    public string? NewPassword { get; set; }
}
