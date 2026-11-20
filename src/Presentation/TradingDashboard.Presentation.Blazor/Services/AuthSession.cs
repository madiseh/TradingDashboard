using System.Net.Http.Headers;

namespace TradingDashboard.Presentation.Blazor.Services;

public sealed class AuthSession
{
    public event Action? Changed;

    public string? AccessToken { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }
    public AuthenticatedUserSession? CurrentUser { get; private set; }
    public bool IsHydrated { get; private set; }

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
        IsHydrated = true;
        AccessToken = authResult.AccessToken;
        ExpiresAtUtc = authResult.ExpiresAtUtc.ToUniversalTime();
        CurrentUser = new AuthenticatedUserSession(
            authResult.UserId ?? string.Empty,
            authResult.Username ?? string.Empty,
            authResult.Email ?? string.Empty,
            authResult.Role ?? string.Empty);
        NotifyChanged();
    }

    public void Clear()
    {
        IsHydrated = true;
        AccessToken = null;
        ExpiresAtUtc = null;
        CurrentUser = null;
        NotifyChanged();
    }

    public void Restore(PersistedAuthSession? session)
    {
        IsHydrated = true;

        if (session is null
            || string.IsNullOrWhiteSpace(session.AccessToken)
            || session.ExpiresAtUtc <= DateTime.UtcNow
            || string.IsNullOrWhiteSpace(session.UserId))
        {
            AccessToken = null;
            ExpiresAtUtc = null;
            CurrentUser = null;
            NotifyChanged();
            return;
        }

        AccessToken = session.AccessToken;
        ExpiresAtUtc = session.ExpiresAtUtc.ToUniversalTime();
        CurrentUser = new AuthenticatedUserSession(
            session.UserId,
            session.Username,
            session.Email,
            session.Role);

        NotifyChanged();
    }

    public void MarkHydrated()
    {
        if (IsHydrated)
        {
            return;
        }

        IsHydrated = true;
        NotifyChanged();
    }

    public PersistedAuthSession? Export()
    {
        if (!IsAuthenticated
            || string.IsNullOrWhiteSpace(AccessToken)
            || ExpiresAtUtc is null
            || CurrentUser is null)
        {
            return null;
        }

        return new PersistedAuthSession(
            AccessToken,
            ExpiresAtUtc.Value,
            CurrentUser.UserId,
            CurrentUser.Username,
            CurrentUser.Email,
            CurrentUser.Role);
    }

    public void Apply(HttpClient client)
    {
        if (!string.IsNullOrWhiteSpace(AccessToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        }
    }

    private void NotifyChanged()
    {
        Changed?.Invoke();
    }

    public sealed record AuthenticatedUserSession(
        string UserId,
        string Username,
        string Email,
        string Role);

    public sealed record PersistedAuthSession(
        string AccessToken,
        DateTime ExpiresAtUtc,
        string UserId,
        string Username,
        string Email,
        string Role);
}
