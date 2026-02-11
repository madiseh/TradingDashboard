using TradingDashboard.Domain.Entities;

namespace TradingDashboard.Application.Repository.Cot;

/// <summary>
/// Repository contract for COT instruments.
/// </summary>
public interface ICotInstrumentRepository : IRepository<CotInstrument, Guid>
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
    /// <param name="group">Group value (for example: Currencies).</param>
    /// <returns>List of instruments in that group.</returns>
    IList<CotInstrument> GetByGroup(string group);
}
