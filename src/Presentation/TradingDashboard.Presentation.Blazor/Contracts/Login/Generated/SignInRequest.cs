using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.Login.Generated;

public sealed class SignInRequest
{
    [JsonPropertyName("identity")]
    public string? Identity { get; set; }

    [JsonPropertyName("password")]
    public string? Password { get; set; }
}
