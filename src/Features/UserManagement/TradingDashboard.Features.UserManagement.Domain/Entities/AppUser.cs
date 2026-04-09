using TradingDashboard.Domain.Common.Common;
using TradingDashboard.Domain.Enums;
using TradingDashboard.Domain.ValueObjects;

namespace TradingDashboard.Domain.Entities;

public sealed class AppUser : BaseAuditableEntity<Guid>
{
    public string Username { get; private set; }
    public EmailAddress Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string PasswordSalt { get; private set; }
    public AppUserRole Role { get; private set; }
    public AppUserStatus Status { get; private set; }
    public SignupAppPlatform SignupAppPlatform { get; private set; }
    public bool IsEmailVerified { get; private set; }
    public DateTime? LastLoginAtUtc { get; private set; }

    public AppUser(
        Guid id,
        string username,
        string email,
        string passwordHash,
        string passwordSalt,
        AppUserRole role,
        AppUserStatus status,
        SignupAppPlatform signupAppPlatform,
        bool isEmailVerified,
        DateTime createdAtUtc)
        : base(id, createdAtUtc)
    {
        Username = NormalizeRequired(username, nameof(username));
        Email = EmailAddress.Create(email);
        PasswordHash = NormalizeRequired(passwordHash, nameof(passwordHash));
        PasswordSalt = NormalizeRequired(passwordSalt, nameof(passwordSalt));
        Role = role;
        Status = status;
        SignupAppPlatform = signupAppPlatform;
        IsEmailVerified = isEmailVerified;
    }

    public void MarkLogin(DateTime loginAtUtc)
    {
        LastLoginAtUtc = loginAtUtc;
        MarkUpdated(loginAtUtc);
    }

    public void ChangePassword(string passwordHash, string passwordSalt, DateTime updatedAtUtc)
    {
        PasswordHash = NormalizeRequired(passwordHash, nameof(passwordHash));
        PasswordSalt = NormalizeRequired(passwordSalt, nameof(passwordSalt));
        MarkUpdated(updatedAtUtc);
    }

    public void SetRole(AppUserRole role, DateTime updatedAtUtc)
    {
        Role = role;
        MarkUpdated(updatedAtUtc);
    }

    public void SetStatus(AppUserStatus status, DateTime updatedAtUtc)
    {
        Status = status;
        MarkUpdated(updatedAtUtc);
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
        Role = AppUserRole.Guest;
        Status = AppUserStatus.Active;
        SignupAppPlatform = SignupAppPlatform.Local;
        IsEmailVerified = false;
    }
}

