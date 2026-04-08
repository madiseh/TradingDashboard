using TradingDashboard.Application.Common.Security;
using TradingDashboard.Domain.Enums;
using TradingDashboard.Domain.ValueObjects;
using TradingDashboard.Infrastructure.Persistence.Implementation.Context;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Seed;

public static class AdminUserSeedBootstrapper
{
    public static int SeedMissingAdmin(UserManagementDbContext dbContext, IPasswordHashingService passwordHashingService)
    {
        if (!EmailAddress.TryCreate(AdminUserSeedData.Email, out var normalizedEmail))
        {
            throw new InvalidOperationException("Admin seed email is invalid.");
        }

        var adminExists = dbContext.AppUsers.Any(x =>
            x.Username == AdminUserSeedData.Username ||
            x.Email == normalizedEmail!);

        if (adminExists)
        {
            return 0;
        }

        var (passwordHash, passwordSalt) = passwordHashingService.CreateHash(AdminUserSeedData.Password);

        var admin = new AppUser(
            Guid.NewGuid(),
            AdminUserSeedData.Username,
            AdminUserSeedData.Email,
            passwordHash,
            passwordSalt,
            SignupAppPlatform.Local,
            false,
            DateTime.UtcNow);

        dbContext.AppUsers.Add(admin);
        return dbContext.SaveChanges();
    }
}
