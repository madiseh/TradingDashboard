using System.Collections.Generic;

namespace TradingDashboard.Domain.Abstraction.Events;

public interface IHasDomainEvents
{
    IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
    void ClearDomainEvents();
}
