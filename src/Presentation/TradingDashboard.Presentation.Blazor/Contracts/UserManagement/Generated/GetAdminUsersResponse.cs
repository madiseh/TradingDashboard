using System.Text.Json.Serialization;

namespace TradingDashboard.Presentation.Blazor.Contracts.UserManagement.Generated;

public sealed class GetAdminUsersResponse
{
    [JsonPropertyName("activeUsers")]
    public int ActiveUsers { get; set; }

    [JsonPropertyName("disabledUsers")]
    public int DisabledUsers { get; set; }

    [JsonPropertyName("superAdminUsers")]
    public int SuperAdminUsers { get; set; }

    [JsonPropertyName("totalUsers")]
    public int TotalUsers { get; set; }

    [JsonPropertyName("users")]
    public IReadOnlyList<AdminUserItemResponse>? Users { get; set; }
}
