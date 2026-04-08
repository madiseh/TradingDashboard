using TradingDashboard.Domain.Entities;

namespace TradingDashboard.Application.Repository.Subscriptions;

public interface ISubscriptionQueryRepository : IQueryRepository<Subscription, Guid>
{
    Task<Subscription?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken);
}
