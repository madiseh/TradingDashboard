using TradingDashboard.Application.Repository.Auth;
using TradingDashboard.Domain.Enums;
using TradingDashboard.Domain.ValueObjects;
using TradingDashboard.Infrastructure.Persistence.Common.Repositories;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

public sealed class AppUserQueryRepository(DbContext context)
    : GenericQueryRepository<AppUser, Guid>(context), IAppUserQueryRepository
{
    public async Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Set<AppUser>()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<AppUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            throw new ArgumentException("Username cannot be empty.", nameof(username));
        }

        var normalized = username.Trim();
        return await _context.Set<AppUser>()
            .FirstOrDefaultAsync(x => x.Username == normalized, cancellationToken);
    }

    public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        if (!EmailAddress.TryCreate(email, out var normalizedEmail))
        {
            throw new ArgumentException("Email is invalid.", nameof(email));
        }

        return await _context.Set<AppUser>()
            .FirstOrDefaultAsync(x => x.Email == normalizedEmail!, cancellationToken);
    }

    public async Task<AppUser?> GetByIdentityAsync(string identity, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(identity))
        {
            throw new ArgumentException("Identity cannot be empty.", nameof(identity));
        }

        var normalized = identity.Trim();

        if (EmailAddress.TryCreate(normalized, out var normalizedEmail))
        {
            return await _context.Set<AppUser>()
                .FirstOrDefaultAsync(x => x.Username == normalized || x.Email == normalizedEmail!, cancellationToken);
        }

        return await _context.Set<AppUser>()
            .FirstOrDefaultAsync(x => x.Username == normalized, cancellationToken);
    }

    public async Task<IReadOnlyList<AppUser>> SearchAsync(string? search, CancellationToken cancellationToken)
    {
        var query = _context.Set<AppUser>()
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim();
            var hasRoleFilter = Enum.TryParse<AppUserRole>(normalized, true, out var parsedRole);
            var hasStatusFilter = Enum.TryParse<AppUserStatus>(normalized, true, out var parsedStatus);

            query = query.Where(x =>
                x.Username.Contains(normalized) ||
                EF.Property<string>(x, nameof(AppUser.Email)).Contains(normalized) ||
                (hasRoleFilter && x.Role == parsedRole) ||
                (hasStatusFilter && x.Status == parsedStatus));
        }

        return await query
            .OrderByDescending(x => x.Role)
            .ThenBy(x => x.Username)
            .ToListAsync(cancellationToken);
    }
}
