using System.Net.Mail;

namespace TradingDashboard.Domain.ValueObjects;

public sealed record EmailAddress
{
    public string Value { get; }

    private EmailAddress(string value)
    {
        Value = value;
    }

    public static EmailAddress Create(string value)
    {
        if (!TryCreate(value, out var email))
        {
            throw new ArgumentException("Email is invalid.", nameof(value));
        }

        return email!;
    }

    public static bool TryCreate(string? value, out EmailAddress? email)
    {
        email = null;
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        var normalized = value.Trim().ToLowerInvariant();

        try
        {
            var parsed = new MailAddress(normalized);
            if (!string.Equals(parsed.Address, normalized, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            email = new EmailAddress(normalized);
            return true;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    public override string ToString()
    {
        return Value;
    }
}
