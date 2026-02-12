using TradingDashboard.Domain.Entities;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Abstractions.Persistence;

public interface ICotInstrumentRepository
{
    Task<bool> ExistsBySlugAsync(string instrumentSlug, CancellationToken cancellationToken);
    Task<CotInstrument?> GetBySlugAsync(string instrumentSlug, CancellationToken cancellationToken);
    Task<IReadOnlyList<CotInstrument>> GetByGroupAsync(CotInstrumentType group, CancellationToken cancellationToken);
}
