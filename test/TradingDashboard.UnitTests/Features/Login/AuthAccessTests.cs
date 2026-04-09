using System.Linq.Expressions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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
        var handler = new SignUpCommandHandler(
            queryRepository,
            commandRepository,
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
            return Task.FromResult<AppUser?>(null);
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
}
