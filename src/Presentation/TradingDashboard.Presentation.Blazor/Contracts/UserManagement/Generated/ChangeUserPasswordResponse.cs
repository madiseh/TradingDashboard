using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Login.Generated;

public sealed class ChangeUserPasswordResponse
{
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }
}
