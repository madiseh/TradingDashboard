namespace TradingDashboard.Domain.Common.Events;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}

