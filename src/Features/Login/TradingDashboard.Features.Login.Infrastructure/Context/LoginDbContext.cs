namespace TradingDashboard.Infrastructure.Persistence.Implementation.Context;

public sealed class LoginDbContext : DbContext
{
    public LoginDbContext(DbContextOptions<LoginDbContext> options) : base(options)
    {
    }

    public DbSet<AppUser> AppUsers => Set<AppUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new AppUserConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
