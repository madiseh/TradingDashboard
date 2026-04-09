namespace TradingDashboard.Domain.Common.Common;

public abstract class BaseEntity<TId> where TId : notnull
{
    public TId Id { get; protected set; }

    protected BaseEntity(TId id)
    {
        Id = id;
    }
}

