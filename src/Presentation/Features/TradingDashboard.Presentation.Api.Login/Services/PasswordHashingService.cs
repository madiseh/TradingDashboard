using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using TradingDashboard.Application.Common.Security;

namespace TradingDashboard.Presentation.Api.Login.Services;

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
        var hashBytes = KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: IterationCount,
            numBytesRequested: SubKeyLength);

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

        var actualHashBytes = KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: IterationCount,
            numBytesRequested: SubKeyLength);

        return CryptographicOperations.FixedTimeEquals(actualHashBytes, expectedHashBytes);
    }
}
