using TradingDashboard.Application.Repository.Subscriptions;
using TradingDashboard.Domain.Enums;
using TradingDashboard.Infrastructure.Persistence.Common.Repositories;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

public sealed class SubscriptionQueryRepository(DbContext context)
    : GenericQueryRepository<Subscription, Guid>(context), ISubscriptionQueryRepository
{
    public async Task<Subscription?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException("UserId cannot be empty.", nameof(userId));
        }

        return await _context.Set<Subscription>()
            .FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
    }
}
