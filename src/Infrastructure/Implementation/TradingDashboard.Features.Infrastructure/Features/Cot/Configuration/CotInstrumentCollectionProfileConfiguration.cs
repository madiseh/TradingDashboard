using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Configuration;

public sealed class CotInstrumentCollectionProfileConfiguration : IEntityTypeConfiguration<CotInstrumentCollectionProfile>
{
    public void Configure(EntityTypeBuilder<CotInstrumentCollectionProfile> entity)
    {
        entity.ToTable(DbTableNames.CotInstrumentCollectionProfiles, DbSchemas.Cot);
        entity.HasKey(x => x.Id);

        entity.Property(x => x.CreatedAtUtc)
            .IsRequired();

        entity.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        entity.Property(x => x.NormalMarketAndExchangeName)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.LegacyMarketAndExchangeName)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.IsNormalEnabled)
            .IsRequired();

        entity.Property(x => x.IsLegacyEnabled)
            .IsRequired();

        entity.HasOne(x => x.Instrument)
            .WithOne()
            .HasForeignKey<CotInstrumentCollectionProfile>(x => x.Id)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
