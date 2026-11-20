using System.Text;

namespace TradingDashboard.Presentation.Api.UserManagement.Options;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "TradingDashboard.UserManagementApi";
    public string Audience { get; init; } = "TradingDashboard.Blazor";
    public string SigningKey { get; init; } = "trading-dashboard-local-dev-signing-key-change-me";
    public int AccessTokenLifetimeMinutes { get; init; } = 60;

    public static JwtOptions ReadAndValidate(IConfiguration configuration)
    {
        var options = configuration.GetSection(SectionName).Get<JwtOptions>() ?? new JwtOptions();
        Validate(options);
        return options;
    }

    public static void Validate(JwtOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Issuer))
        {
            throw new InvalidOperationException("Jwt:Issuer must be configured.");
        }

        if (string.IsNullOrWhiteSpace(options.Audience))
        {
            throw new InvalidOperationException("Jwt:Audience must be configured.");
        }

        if (string.IsNullOrWhiteSpace(options.SigningKey))
        {
            throw new InvalidOperationException("Jwt:SigningKey must be configured.");
        }

        if (Encoding.UTF8.GetByteCount(options.SigningKey) < 32)
        {
            throw new InvalidOperationException("Jwt:SigningKey must be at least 32 bytes for HMAC-SHA256.");
        }

        if (options.AccessTokenLifetimeMinutes <= 0)
        {
            throw new InvalidOperationException("Jwt:AccessTokenLifetimeMinutes must be greater than zero.");
        }
    }
}
