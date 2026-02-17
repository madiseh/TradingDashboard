using TradingDashboard.Application.Repository.Auth;
using TradingDashboard.Infrastructure.Persistence.Common.Repositories;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

public sealed class AppUserCommandRepository(DbContext context)
    : GenericCommandRepository<AppUser>(context), IAppUserCommandRepository
{
}
