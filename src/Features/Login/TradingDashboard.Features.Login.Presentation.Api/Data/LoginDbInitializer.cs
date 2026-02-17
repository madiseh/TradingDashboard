using Microsoft.Extensions.Options;
using TradingDashboard.Application.Common.Security;
using TradingDashboard.Domain.ValueObjects;
using TradingDashboard.Presentation.Api.Login.Options;

namespace TradingDashboard.Presentation.Api.Login.Data;

public static class LoginDbInitializer
{
    public static async Task InitializeAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        var dbContext = serviceProvider.GetRequiredService<LoginDbContext>();
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        var adminSeedOptions = serviceProvider
            .GetRequiredService<IOptions<AdminSeedOptions>>()
            .Value;

        if (!adminSeedOptions.Enabled || string.IsNullOrWhiteSpace(adminSeedOptions.Password))
        {
            return;
        }

        var username = adminSeedOptions.Username.Trim();
        var email = adminSeedOptions.Email.Trim();
        if (!EmailAddress.TryCreate(email, out var normalizedEmail))
        {
            throw new InvalidOperationException("AdminSeed.Email is invalid.");
        }

        var adminExists = await dbContext.AppUsers.AnyAsync(
            x => x.Username == username || x.Email == normalizedEmail!,
            cancellationToken);

        if (adminExists)
        {
            return;
        }

        var passwordHashingService = serviceProvider.GetRequiredService<IPasswordHashingService>();
        var (passwordHash, passwordSalt) = passwordHashingService.CreateHash(adminSeedOptions.Password);

        var adminUser = new AppUser(
            Guid.NewGuid(),
            username,
            email,
            passwordHash,
            passwordSalt,
            DateTime.UtcNow);

        dbContext.AppUsers.Add(adminUser);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
