using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Configuration;

public sealed class CotInstrumentConfiguration : IEntityTypeConfiguration<CotInstrument>
{
    public void Configure(EntityTypeBuilder<CotInstrument> entity)
    {
        entity.ToTable(DbTableNames.CotInstruments, DbSchemas.Cot);
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
            .HasConversion<int>()
            .IsRequired();

        entity.HasIndex(x => x.InstrumentId).IsUnique();
        entity.HasIndex(x => x.InstrumentSlug).IsUnique();
        entity.HasIndex(x => x.Type);

        entity.HasOne(x => x.TypeLookup)
            .WithMany()
            .HasForeignKey(x => x.Type)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
