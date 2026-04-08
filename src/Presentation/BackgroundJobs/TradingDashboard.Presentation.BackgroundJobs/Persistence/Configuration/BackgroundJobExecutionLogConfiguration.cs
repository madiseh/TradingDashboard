using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Constants;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Entities;

namespace TradingDashboard.Presentation.BackgroundJobs.Persistence.Configuration;

public sealed class BackgroundJobExecutionLogConfiguration : IEntityTypeConfiguration<BackgroundJobExecutionLog>
{
    public void Configure(EntityTypeBuilder<BackgroundJobExecutionLog> entity)
    {
        entity.ToTable(DbTableNames.BackgroundJobExecutionLogs, DbSchemas.BackgroundJobs);
        entity.HasKey(x => x.Id);

        entity.Property(x => x.JobName)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.JobGroup)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.TriggerName)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.TriggerGroup)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.IsSuccessful)
            .IsRequired();

        entity.Property(x => x.ExceptionType)
            .HasMaxLength(512);

        entity.Property(x => x.ErrorMessage)
            .HasMaxLength(4000);

        entity.Property(x => x.ExecutedAtUtc)
            .IsRequired();
    }
}
