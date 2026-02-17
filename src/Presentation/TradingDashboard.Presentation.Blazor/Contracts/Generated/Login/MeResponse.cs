using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Generated.Login;

public sealed class MeResponse
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("userId")]
    public string? UserId { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }
}

