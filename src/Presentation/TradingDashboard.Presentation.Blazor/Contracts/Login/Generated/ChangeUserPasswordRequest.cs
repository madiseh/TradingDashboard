using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Login.Generated;

public sealed class ChangeUserPasswordRequest
{
    [JsonPropertyName("newPassword")]
    public string? NewPassword { get; set; }
}
