using TradingDashboard.Application.Repository.Subscriptions;
using TradingDashboard.Infrastructure.Persistence.Common.Repositories;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

public sealed class SubscriptionCommandRepository(DbContext context)
    : GenericCommandRepository<Subscription>(context), ISubscriptionCommandRepository
{
}
