using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.UserManagement.Generated;

public sealed class ApiErrorResponse
{
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    [JsonPropertyName("errors")]
    public IReadOnlyList<string>? Errors { get; set; }

    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
