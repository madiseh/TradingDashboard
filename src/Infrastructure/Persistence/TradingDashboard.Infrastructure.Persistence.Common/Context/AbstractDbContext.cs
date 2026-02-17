

using Microsoft.EntityFrameworkCore;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Context;

public sealed class AbstractDbContext : DbContext
{
    public AbstractDbContext(DbContextOptions<AbstractDbContext> options) : base(options)
    {
    }

   
}
