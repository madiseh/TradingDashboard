using System.Linq.Expressions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TradingDashboard.Application.Common.Notifications;
using TradingDashboard.Application.Features.Auth.Commands.ForgotPassword;
using TradingDashboard.Application.Features.Auth.Commands.ResendActivationEmail;
using TradingDashboard.Application.Features.Auth.Commands.SignIn;
using TradingDashboard.Application.Common.Security;
using TradingDashboard.Application.Features.Auth.Commands.SignUp;
using TradingDashboard.Application.Repository.Auth;
using TradingDashboard.Domain.Entities;
using TradingDashboard.Domain.Enums;
using TradingDashboard.Infrastructure.Persistence.Implementation.Context;
using TradingDashboard.Infrastructure.Persistence.Implementation.Seed;
using TradingDashboard.Presentation.Api.UserManagement.Services;

namespace TradingDashboard.UnitTests.Features.Login;

public sealed class AuthAccessTests
{
    [Fact]
    public async Task SignUp_CreatesGuestRole_ByDefault()
    {
        var queryRepository = new FakeAppUserQueryRepository();
        var commandRepository = new FakeAppUserCommandRepository();
        var notificationService = new FakeAuthNotificationService();
        var handler = new SignUpCommandHandler(
            queryRepository,
            commandRepository,
            notificationService,
            new PasswordHashingService(),
            new SignUpCommandValidator());

        var result = await handler.Handle(
            new SignUpCommand("guest-user", "guest@example.com", "Password@123"),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotNull(commandRepository.CreatedUser);
        Assert.Equal(AppUserRole.Guest, commandRepository.CreatedUser!.Role);
        Assert.Equal(AppUserRole.Guest, result.Data!.Role);
        Assert.Single(notificationService.ActivationEmailUserIds);
    }

    [Fact]
    public async Task SignIn_UnverifiedLocalUser_ReturnsEmailNotVerified()
    {
        var passwordHashingService = new PasswordHashingService();
        var (passwordHash, passwordSalt) = passwordHashingService.CreateHash("Password@123");
        var user = new AppUser(
            Guid.NewGuid(),
            "guest-user",
            "guest@example.com",
            passwordHash,
            passwordSalt,
            AppUserRole.Guest,
            AppUserStatus.Active,
            SignupAppPlatform.Local,
            false,
            DateTime.UtcNow);

        var queryRepository = new FakeAppUserQueryRepository
        {
            IdentityUser = user
        };

        var handler = new SignInCommandHandler(
            queryRepository,
            new FakeAppUserCommandRepository(),
            passwordHashingService,
            new SignInCommandValidator());

        var result = await handler.Handle(
            new SignInCommand("guest-user", "Password@123"),
            CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("EMAIL_NOT_VERIFIED", result.ErrorCode);
        Assert.Contains(result.Errors, error => error.Contains("activation", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task ForgotPassword_VerifiedUser_SendsResetNotification()
    {
        var user = CreateUser(isEmailVerified: true);
        var queryRepository = new FakeAppUserQueryRepository
        {
            IdentityUser = user
        };
        var notificationService = new FakeAuthNotificationService();
        var handler = new ForgotPasswordCommandHandler(
            queryRepository,
            notificationService,
            new ForgotPasswordCommandValidator());

        var result = await handler.Handle(
            new ForgotPasswordCommand(user.Username),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(notificationService.PasswordResetUserIds);
        Assert.Equal(user.Id, notificationService.PasswordResetUserIds[0]);
    }

    [Fact]
    public async Task ResendActivationEmail_UnverifiedLocalUser_SendsActivationNotification()
    {
        var user = CreateUser(isEmailVerified: false);
        var queryRepository = new FakeAppUserQueryRepository
        {
            IdentityUser = user
        };
        var notificationService = new FakeAuthNotificationService();
        var handler = new ResendActivationEmailCommandHandler(
            queryRepository,
            notificationService,
            new ResendActivationEmailCommandValidator());

        var result = await handler.Handle(
            new ResendActivationEmailCommand(user.Username),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(notificationService.ActivationEmailUserIds);
        Assert.Equal(user.Id, notificationService.ActivationEmailUserIds[0]);
    }

    [Fact]
    public void AdminSeed_CreatesSuperAdmin_Role()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<UserManagementDbContext>()
            .UseSqlite(connection)
            .Options;

        using var dbContext = new UserManagementDbContext(options);
        dbContext.Database.EnsureCreated();

        var affectedRows = AdminUserSeedBootstrapper.SeedMissingAdmin(dbContext, new PasswordHashingService());
        var admin = dbContext.AppUsers.Single();

        Assert.Equal(1, affectedRows);
        Assert.Equal(AdminUserSeedData.Username, admin.Username);
        Assert.Equal(AppUserRole.SuperAdmin, admin.Role);
    }

    private sealed class FakeAppUserQueryRepository : IAppUserQueryRepository
    {
        public AppUser? IdentityUser { get; init; }

        public AppUser Get(Guid id)
        {
            throw new NotSupportedException();
        }

        public IList<AppUser> GetList()
        {
            return [];
        }

        public bool Exists(Expression<Func<AppUser, bool>> expression)
        {
            return false;
        }

        public Task<AppUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
        {
            return Task.FromResult<AppUser?>(null);
        }

        public Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return Task.FromResult<AppUser?>(null);
        }

        public Task<AppUser?> GetByIdentityAsync(string identity, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityUser);
        }

        public Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return Task.FromResult<AppUser?>(null);
        }

        public Task<IReadOnlyList<AppUser>> SearchAsync(string? search, CancellationToken cancellationToken)
        {
            IReadOnlyList<AppUser> users = [];
            return Task.FromResult(users);
        }
    }

    private sealed class FakeAppUserCommandRepository : IAppUserCommandRepository
    {
        public AppUser? CreatedUser { get; private set; }

        public void Create(AppUser entity)
        {
            CreatedUser = entity;
        }

        public void SaveChanges()
        {
        }
    }

    private sealed class FakeAuthNotificationService : IAuthNotificationService
    {
        public List<Guid> ActivationEmailUserIds { get; } = [];
        public List<Guid> PasswordResetUserIds { get; } = [];

        public Task SendActivationEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            ActivationEmailUserIds.Add(user.Id);
            return Task.CompletedTask;
        }

        public Task SendPasswordResetEmailAsync(AppUser user, CancellationToken cancellationToken)
        {
            PasswordResetUserIds.Add(user.Id);
            return Task.CompletedTask;
        }
    }

    private static AppUser CreateUser(bool isEmailVerified)
    {
        var passwordHashingService = new PasswordHashingService();
        var (passwordHash, passwordSalt) = passwordHashingService.CreateHash("Password@123");

        return new AppUser(
            Guid.NewGuid(),
            "guest-user",
            "guest@example.com",
            passwordHash,
            passwordSalt,
            AppUserRole.Guest,
            AppUserStatus.Active,
            SignupAppPlatform.Local,
            isEmailVerified,
            DateTime.UtcNow);
    }
}
