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
    /// <returns>The instrument when found; otherwise null.</returns>
    CotInstrument? GetBySlug(string instrumentSlug);

    /// <summary>
    /// Returns instruments by market group/type.
    /// </summary>
    /// <param name="group">Group value.</param>
    /// <returns>List of instruments in that group.</returns>
    IList<CotInstrument> GetByGroup(CotInstrumentType group);
}
