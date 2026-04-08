using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Login.Generated;

public sealed class AuthTokenResult
{
    [JsonPropertyName("accessToken")]
    public string? AccessToken { get; set; }

    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("expiresAtUtc")]
    public DateTime ExpiresAtUtc { get; set; }

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("role")]
    public string? Role { get; set; }
}
