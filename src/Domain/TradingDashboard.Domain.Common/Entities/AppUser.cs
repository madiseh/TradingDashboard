using TradingDashboard.Domain.Common.Common;
using TradingDashboard.Domain.ValueObjects;

namespace TradingDashboard.Domain.Entities;

public sealed class AppUser : BaseAuditableEntity<Guid>
{
    public string Username { get; private set; }
    public EmailAddress Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string PasswordSalt { get; private set; }
    public DateTime? LastLoginAtUtc { get; private set; }

    public AppUser(
        Guid id,
        string username,
        string email,
        string passwordHash,
        string passwordSalt,
        DateTime createdAtUtc)
        : base(id, createdAtUtc)
    {
        Username = NormalizeRequired(username, nameof(username));
        Email = EmailAddress.Create(email);
        PasswordHash = NormalizeRequired(passwordHash, nameof(passwordHash));
        PasswordSalt = NormalizeRequired(passwordSalt, nameof(passwordSalt));
    }

    public void MarkLogin(DateTime loginAtUtc)
    {
        LastLoginAtUtc = loginAtUtc;
        MarkUpdated(loginAtUtc);
    }

    private static string NormalizeRequired(string value, string argumentName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"{argumentName} cannot be empty.", argumentName);
        }

        return value.Trim();
    }

    private AppUser() : base(Guid.Empty, DateTime.UtcNow)
    {
        Username = string.Empty;
        Email = EmailAddress.Create("placeholder@example.local");
        PasswordHash = string.Empty;
        PasswordSalt = string.Empty;
    }
}

