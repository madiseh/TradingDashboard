using System.Net.Http.Headers;

namespace TradingDashboard.Presentation.Blazor.Services;

public sealed class AuthSession
{
    public string? AccessToken { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public AuthenticatedUserSession? CurrentUser { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AccessToken)
        && ExpiresAtUtc is not null
        && ExpiresAtUtc > DateTime.UtcNow
        && CurrentUser is not null;

    public string? UserId => CurrentUser?.UserId;
    public string? Username => CurrentUser?.Username;
    public string? Email => CurrentUser?.Email;
    public string? Role => CurrentUser?.Role;

    public void SetAuthenticatedUser(AuthTokenResult authResult)
    {
        AccessToken = authResult.AccessToken;
        ExpiresAtUtc = authResult.ExpiresAtUtc.ToUniversalTime();
        CurrentUser = new AuthenticatedUserSession(
            authResult.UserId ?? string.Empty,
            authResult.Username ?? string.Empty,
            authResult.Email ?? string.Empty,
            authResult.Role ?? string.Empty);
    }

    public void Clear()
    {
        AccessToken = null;
        ExpiresAtUtc = null;
        CurrentUser = null;
    }

    public void Apply(HttpClient client)
    {
        if (!string.IsNullOrWhiteSpace(AccessToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        }
    }

    public sealed record AuthenticatedUserSession(
        string UserId,
        string Username,
        string Email,
        string Role);
}
