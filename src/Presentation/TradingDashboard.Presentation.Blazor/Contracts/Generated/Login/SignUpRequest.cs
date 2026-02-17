using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Generated.Login;

public sealed class SignUpRequest
{
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }
}

