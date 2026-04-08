using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Constants;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Entities;

namespace TradingDashboard.Presentation.BackgroundJobs.Persistence.Configuration;

public sealed class BackgroundJobRecordConfiguration : IEntityTypeConfiguration<BackgroundJobRecord>
{
    public void Configure(EntityTypeBuilder<BackgroundJobRecord> entity)
    {
        entity.ToTable(DbTableNames.BackgroundJobs, DbSchemas.BackgroundJobs);
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .HasMaxLength(32)
            .IsRequired();

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

        entity.Property(x => x.Status)
            .HasMaxLength(64)
            .HasDefaultValue(BackgroundJobStatuses.Running)
            .IsRequired();

        entity.Property(x => x.JobType)
            .HasMaxLength(512)
            .IsRequired();

        entity.Property(x => x.CreatedAtUtc)
            .IsRequired();

        entity.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        entity.HasIndex(x => new { x.JobName, x.JobGroup })
            .IsUnique();
    }
}
