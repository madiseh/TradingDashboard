using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Configuration;

public sealed class CotInstrumentTypeLookupConfiguration : IEntityTypeConfiguration<CotInstrumentTypeLookup>
{
    public void Configure(EntityTypeBuilder<CotInstrumentTypeLookup> entity)
    {
        entity.ToTable("CotInstrumentTypes");
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .HasConversion<int>()
            .ValueGeneratedNever();

        entity.Property(x => x.Name)
            .HasMaxLength(128)
            .UseCollation("NOCASE")
            .IsRequired();

        entity.HasIndex(x => x.Name).IsUnique();
    }
}
