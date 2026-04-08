namespace TradingDashboard.Domain.Common.Common;

public abstract class BaseAuditableEntity<TId> : BaseEntity<TId> where TId : notnull
{
    public DateTime CreatedAtUtc { get; protected set; }
    public DateTime UpdatedAtUtc { get; protected set; }

    protected BaseAuditableEntity(TId id, DateTime createdAtUtc) : base(id)
    {
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    protected void MarkUpdated(DateTime updatedAtUtc)
    {
        UpdatedAtUtc = updatedAtUtc;
    }
}

