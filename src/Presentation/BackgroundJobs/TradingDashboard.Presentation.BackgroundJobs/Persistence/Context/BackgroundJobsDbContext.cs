using Microsoft.EntityFrameworkCore;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Configuration;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Entities;

namespace TradingDashboard.Presentation.BackgroundJobs.Persistence.Context;

public sealed class BackgroundJobsDbContext : DbContext
{
    public BackgroundJobsDbContext(DbContextOptions<BackgroundJobsDbContext> options) : base(options)
    {
    }

    public DbSet<BackgroundJobRecord> BackgroundJobs => Set<BackgroundJobRecord>();
    public DbSet<BackgroundJobExecutionLog> BackgroundJobExecutionLogs => Set<BackgroundJobExecutionLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new BackgroundJobRecordConfiguration());
        modelBuilder.ApplyConfiguration(new BackgroundJobExecutionLogConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
