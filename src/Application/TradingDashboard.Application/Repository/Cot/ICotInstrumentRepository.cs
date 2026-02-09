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
}
