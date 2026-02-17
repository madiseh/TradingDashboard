using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TradingDashboard.Domain.Common.Common;

namespace TradingDashboard.Infrastructure.Persistence.Common.Configuration;

public static class BaseAuditableEntityConfiguration
{
    public static void ConfigureAuditable<TId>(this EntityTypeBuilder<BaseAuditableEntity<TId>> entity)
        where TId : notnull
    {
        entity.Property(x => x.CreatedAtUtc).IsRequired();
        entity.Property(x => x.UpdatedAtUtc).IsRequired();
    }
}

