using TradingDashboard.Domain.Entities;

namespace TradingDashboard.Application.Repository.Auth;

public interface IAppUserQueryRepository : IQueryRepository<AppUser, Guid>
{
    Task<AppUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<AppUser?> GetByUsernameAsync(string username, CancellationToken cancellationToken);
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<AppUser?> GetByIdentityAsync(string identity, CancellationToken cancellationToken);
    Task<IReadOnlyList<AppUser>> SearchAsync(string? search, CancellationToken cancellationToken);
}
