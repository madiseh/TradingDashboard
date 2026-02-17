using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingDashboard.Domain.ValueObjects;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Configuration;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> entity)
    {
        entity.ToTable(DbTableNames.AppUsers, DbSchemas.Auth);
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .IsRequired();

        entity.Property(x => x.Username)
            .HasMaxLength(64)
            .IsRequired();

        entity.Property(x => x.Email)
            .HasConversion(
                x => x.Value,
                value => EmailAddress.Create(value))
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.PasswordHash)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.PasswordSalt)
            .HasMaxLength(256)
            .IsRequired();

        entity.Property(x => x.LastLoginAtUtc)
            .IsRequired(false);

        entity.Property(x => x.CreatedAtUtc).IsRequired();
        entity.Property(x => x.UpdatedAtUtc).IsRequired();

        entity.HasIndex(x => x.Username)
            .IsUnique();

        entity.HasIndex(x => x.Email)
            .IsUnique();
    }
}
