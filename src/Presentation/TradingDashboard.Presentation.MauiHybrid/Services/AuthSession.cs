namespace TradingDashboard.Presentation.MauiHybrid.Services;

public sealed class AuthSession
{
    public string? AccessToken { get; private set; }
    public DateTime? ExpiresAtUtc { get; private set; }

    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(AccessToken)
        && ExpiresAtUtc is not null
        && ExpiresAtUtc > DateTime.UtcNow;

    public void SetToken(string accessToken, DateTime expiresAtUtc)
    {
        AccessToken = accessToken;
        ExpiresAtUtc = expiresAtUtc;
    }

    public void Clear()
    {
        AccessToken = null;
        ExpiresAtUtc = null;
    }

    public void Apply(HttpClient client)
    {
        if (!string.IsNullOrWhiteSpace(AccessToken))
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        }
    }
}
