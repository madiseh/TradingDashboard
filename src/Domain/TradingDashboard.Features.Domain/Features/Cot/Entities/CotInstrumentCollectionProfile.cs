using TradingDashboard.Domain.Common.Common;

namespace TradingDashboard.Domain.Entities;

public sealed class CotInstrumentCollectionProfile : BaseAuditableEntity<Guid>
{
    public string NormalMarketAndExchangeName { get; private set; }
    public string LegacyMarketAndExchangeName { get; private set; }
    public bool IsNormalEnabled { get; private set; }
    public bool IsLegacyEnabled { get; private set; }
    public CotInstrument? Instrument { get; private set; }

    public CotInstrumentCollectionProfile(
        Guid id,
        string normalMarketAndExchangeName,
        string legacyMarketAndExchangeName,
        bool isNormalEnabled,
        bool isLegacyEnabled,
        DateTime createdAtUtc)
        : base(id, createdAtUtc)
    {
        NormalMarketAndExchangeName = ValidateMarketName(normalMarketAndExchangeName, nameof(normalMarketAndExchangeName));
        LegacyMarketAndExchangeName = ValidateMarketName(legacyMarketAndExchangeName, nameof(legacyMarketAndExchangeName));
        IsNormalEnabled = isNormalEnabled;
        IsLegacyEnabled = isLegacyEnabled;
    }

    private CotInstrumentCollectionProfile() : base(Guid.Empty, DateTime.UtcNow)
    {
        NormalMarketAndExchangeName = string.Empty;
        LegacyMarketAndExchangeName = string.Empty;
    }

    public void Refresh(
        string normalMarketAndExchangeName,
        string legacyMarketAndExchangeName,
        bool isNormalEnabled,
        bool isLegacyEnabled,
        DateTime updatedAtUtc)
    {
        NormalMarketAndExchangeName = ValidateMarketName(normalMarketAndExchangeName, nameof(normalMarketAndExchangeName));
        LegacyMarketAndExchangeName = ValidateMarketName(legacyMarketAndExchangeName, nameof(legacyMarketAndExchangeName));
        IsNormalEnabled = isNormalEnabled;
        IsLegacyEnabled = isLegacyEnabled;
        MarkUpdated(updatedAtUtc);
    }

    private static string ValidateMarketName(string value, string paramName)
    {
        return string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Market and exchange name cannot be empty.", paramName)
            : value.Trim();
    }
}
