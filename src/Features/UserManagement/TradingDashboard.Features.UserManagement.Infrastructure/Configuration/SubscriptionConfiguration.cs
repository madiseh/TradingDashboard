using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Infrastructure.Persistence.Implementation.Configuration;

public sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> entity)
    {
        entity.ToTable(DbTableNames.Subscriptions, DbSchemas.Auth);
        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .IsRequired();

        entity.Property(x => x.UserId)
            .IsRequired();

        entity.Property(x => x.PlanCode)
            .HasMaxLength(64)
            .IsRequired();

        entity.Property(x => x.PlanName)
            .HasMaxLength(128)
            .IsRequired();

        entity.Property(x => x.BillingCycle)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        entity.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        entity.Property(x => x.BasePriceUsd)
            .HasPrecision(18, 2)
            .IsRequired();

        entity.Property(x => x.DiscountPercentage)
            .HasPrecision(18, 2)
            .IsRequired();

        entity.Property(x => x.FinalPriceUsd)
            .HasPrecision(18, 2)
            .IsRequired();

        entity.Property(x => x.StartsAtUtc)
            .IsRequired();

        entity.Property(x => x.ExpiresAtUtc)
            .IsRequired();

        entity.Property(x => x.PurchasedAtUtc)
            .IsRequired();

        entity.Property(x => x.CreatedAtUtc)
            .IsRequired();

        entity.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        entity.HasIndex(x => x.UserId)
            .IsUnique();

        entity.HasOne<AppUser>()
            .WithOne()
            .HasForeignKey<Subscription>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
