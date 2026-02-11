using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Configuration;

public sealed class CotInstrumentConfiguration : IEntityTypeConfiguration<CotInstrument>
{
    public void Configure(EntityTypeBuilder<CotInstrument> entity)
    {
        entity.ToTable("CotInstruments");
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .IsRequired();

        entity.Property(x => x.InstrumentId)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(x => x.InstrumentSlug)
            .HasMaxLength(256)
            .UseCollation("NOCASE")
            .IsRequired();

        entity.Property(x => x.InstrumentName)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.Type)
            .HasMaxLength(128)
            .UseCollation("NOCASE")
            .IsRequired();

        entity.HasIndex(x => x.InstrumentId).IsUnique();
        entity.HasIndex(x => x.InstrumentSlug).IsUnique();
        entity.HasIndex(x => x.Type);
    }
}
