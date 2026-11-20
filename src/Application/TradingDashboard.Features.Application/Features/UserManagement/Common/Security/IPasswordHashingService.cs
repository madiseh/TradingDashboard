namespace TradingDashboard.Application.Common.Security;

public interface IPasswordHashingService
{
    (string PasswordHash, string PasswordSalt) CreateHash(string password);
    bool Verify(string password, string passwordHash, string passwordSalt);
}
