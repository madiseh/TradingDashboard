using System.Security.Cryptography;
using TradingDashboard.Application.Common.Security;

namespace TradingDashboard.Presentation.Api.UserManagement.Services;

public sealed class PasswordHashingService : IPasswordHashingService
{
    private const int SaltSize = 16;
    private const int SubKeyLength = 32;
    private const int IterationCount = 100_000;

    public (string PasswordHash, string PasswordSalt) CreateHash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be empty.", nameof(password));
        }

        var saltBytes = RandomNumberGenerator.GetBytes(SaltSize);
        var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            IterationCount,
            HashAlgorithmName.SHA256,
            SubKeyLength);

        return (Convert.ToBase64String(hashBytes), Convert.ToBase64String(saltBytes));
    }

    public bool Verify(string password, string passwordHash, string passwordSalt)
    {
        if (string.IsNullOrWhiteSpace(password)
            || string.IsNullOrWhiteSpace(passwordHash)
            || string.IsNullOrWhiteSpace(passwordSalt))
        {
            return false;
        }

        byte[] saltBytes;
        byte[] expectedHashBytes;

        try
        {
            saltBytes = Convert.FromBase64String(passwordSalt);
            expectedHashBytes = Convert.FromBase64String(passwordHash);
        }
        catch (FormatException)
        {
            return false;
        }

        var actualHashBytes = Rfc2898DeriveBytes.Pbkdf2(
            password,
            saltBytes,
            IterationCount,
            HashAlgorithmName.SHA256,
            SubKeyLength);

        return CryptographicOperations.FixedTimeEquals(actualHashBytes, expectedHashBytes);
    }
}
