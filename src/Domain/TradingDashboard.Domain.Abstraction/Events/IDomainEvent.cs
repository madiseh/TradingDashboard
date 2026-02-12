namespace TradingDashboard.Domain.Abstraction.Events;

public interface IDomainEvent
{
    DateTime OccurredOnUtc { get; }
}
