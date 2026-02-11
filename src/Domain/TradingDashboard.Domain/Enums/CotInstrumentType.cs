namespace TradingDashboard.Domain.Enums;

public enum CotInstrumentType
{
    Currencies = 1,
    Cryptocurrencies = 2,
    Indexes = 3,
    TreasuriesAndRates = 4,
    Energies = 5,
    Grains = 6,
    LivestockAndDairy = 7,
    Metals = 8,
    Softs = 9
}

public static class CotInstrumentTypeExtensions
{
    public static string ToContractValue(this CotInstrumentType type)
    {
        return type switch
        {
            CotInstrumentType.Currencies => "Currencies",
            CotInstrumentType.Cryptocurrencies => "Cryptocurrencies",
            CotInstrumentType.Indexes => "Indexes",
            CotInstrumentType.TreasuriesAndRates => "Treasuries and Rates",
            CotInstrumentType.Energies => "Energies",
            CotInstrumentType.Grains => "Grains",
            CotInstrumentType.LivestockAndDairy => "Livestock & Dairy",
            CotInstrumentType.Metals => "Metals",
            CotInstrumentType.Softs => "Softs",
            _ => "Currencies"
        };
    }

    public static bool TryParse(string? value, out CotInstrumentType type)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            type = CotInstrumentType.Currencies;
            return false;
        }

        var normalized = value.Trim().ToLowerInvariant();
        switch (normalized)
        {
            case "currencies":
                type = CotInstrumentType.Currencies;
                return true;
            case "cryptocurrencies":
                type = CotInstrumentType.Cryptocurrencies;
                return true;
            case "indexes":
                type = CotInstrumentType.Indexes;
                return true;
            case "treasuries and rates":
            case "treasuriesandrates":
                type = CotInstrumentType.TreasuriesAndRates;
                return true;
            case "energies":
                type = CotInstrumentType.Energies;
                return true;
            case "grains":
                type = CotInstrumentType.Grains;
                return true;
            case "livestock & dairy":
            case "livestock and dairy":
            case "livestockanddairy":
                type = CotInstrumentType.LivestockAndDairy;
                return true;
            case "metals":
                type = CotInstrumentType.Metals;
                return true;
            case "softs":
                type = CotInstrumentType.Softs;
                return true;
        }

        if (Enum.TryParse(value, true, out CotInstrumentType parsed) &&
            Enum.IsDefined(parsed))
        {
            type = parsed;
            return true;
        }

        type = CotInstrumentType.Currencies;
        return false;
    }

    public static CotInstrumentType Parse(string value)
    {
        if (TryParse(value, out var type))
        {
            return type;
        }

        throw new ArgumentException($"Unsupported market type '{value}'.", nameof(value));
    }
}
