using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Configuration;

public sealed class CotCollectionJobStateConfiguration : IEntityTypeConfiguration<CotCollectionJobState>
{
    public void Configure(EntityTypeBuilder<CotCollectionJobState> entity)
    {
        entity.ToTable(DbTableNames.CotCollectionJobStates, DbSchemas.Cot);
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(x => x.CreatedAtUtc)
            .IsRequired();

        entity.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        entity.Property(x => x.LastError)
            .HasMaxLength(2048);
    }
}
