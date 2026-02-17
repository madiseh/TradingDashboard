using TradingDashboard.Domain.Entities;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Repository.Cot;

/// <summary>
/// Query repository contract for COT instruments.
/// </summary>
public interface ICotInstrumentQueryRepository : IQueryRepository<CotInstrument, Guid>
{
    /// <summary>
    /// Returns an instrument by slug.
    /// </summary>
    /// <param name="instrumentSlug">Slug value (for example: australian-dollar).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The instrument when found; otherwise null.</returns>
    Task<CotInstrument?> GetBySlugAsync(string instrumentSlug, CancellationToken cancellationToken);

    /// <summary>
    /// Returns instruments by market group/type.
    /// </summary>
    /// <param name="group">Group value.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of instruments in that group.</returns>
    Task<IReadOnlyList<CotInstrument>> GetByGroupAsync(CotInstrumentType group, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all instruments ordered by group and instrument name.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Ordered list of all instruments.</returns>
    Task<IReadOnlyList<CotInstrument>> GetAllOrderedAsync(CancellationToken cancellationToken);
}
