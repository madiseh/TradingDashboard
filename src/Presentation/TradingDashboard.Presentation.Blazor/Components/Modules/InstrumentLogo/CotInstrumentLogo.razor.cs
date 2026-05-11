using Microsoft.AspNetCore.Components;

namespace TradingDashboard.Presentation.Blazor.Components.Modules.InstrumentLogo;

public partial class CotInstrumentLogo
{
    private const string FlagCdnBaseUrl = "https://flagcdn.com/w80";
    private const string CryptoLogosBaseUrl = "https://cryptologos.cc/logos";

    private static readonly IReadOnlyDictionary<string, InstrumentLogoAsset> LogoAssets =
        new Dictionary<string, InstrumentLogoAsset>(StringComparer.OrdinalIgnoreCase)
        {
            // Currencies
            ["australian-dollar"] = new($"{FlagCdnBaseUrl}/au.png", "AUD", "flag"),
            ["brazilian-real"] = new($"{FlagCdnBaseUrl}/br.png", "BRL", "flag"),
            ["british-pound-sterling"] = new($"{FlagCdnBaseUrl}/gb.png", "GBP", "flag"),
            ["canadian-dollar"] = new($"{FlagCdnBaseUrl}/ca.png", "CAD", "flag"),
            ["euro-fx"] = new($"{FlagCdnBaseUrl}/eu.png", "EUR", "flag"),
            ["japanese-yen"] = new($"{FlagCdnBaseUrl}/jp.png", "JPY", "flag"),
            ["mexican-peso"] = new($"{FlagCdnBaseUrl}/mx.png", "MXN", "flag"),
            ["new-zealand-dollar"] = new($"{FlagCdnBaseUrl}/nz.png", "NZD", "flag"),
            ["south-african-rand"] = new($"{FlagCdnBaseUrl}/za.png", "ZAR", "flag"),
            ["swiss-franc"] = new($"{FlagCdnBaseUrl}/ch.png", "CHF", "flag"),
            ["us-dollar-index"] = new($"{FlagCdnBaseUrl}/us.png", "DXY", "flag"),

            // Cryptocurrencies
            ["bitcoin"] = new($"{CryptoLogosBaseUrl}/bitcoin-btc-logo.svg?v=040", "BTC", "crypto"),
            ["micro-bitcoin"] = new($"{CryptoLogosBaseUrl}/bitcoin-btc-logo.svg?v=040", "MBT", "crypto"),
            ["ether-cash-settled"] = new($"{CryptoLogosBaseUrl}/ethereum-eth-logo.svg?v=040", "ETH", "crypto"),
            ["micro-ether"] = new($"{CryptoLogosBaseUrl}/ethereum-eth-logo.svg?v=040", "MET", "crypto"),

            // Indexes
            ["sp-500-stock-index"] = Badge("SPX", "#1d4ed8", "#dbeafe"),
            ["nasdaq-100-stock-index-mini"] = Badge("NDX", "#0f766e", "#ccfbf1"),
            ["dow-jones-industrial-avg-x5"] = Badge("DJI", "#4338ca", "#e0e7ff"),
            ["russell-2000-mini-index-future"] = Badge("RUT", "#be123c", "#ffe4e6"),
            ["e-mini-sp-400-stock-index"] = Badge("MID", "#7c2d12", "#ffedd5"),
            ["e-mini-sp-500-stock-index"] = Badge("ES", "#1d4ed8", "#dbeafe"),
            ["vix-futures"] = Badge("VIX", "#581c87", "#f3e8ff"),

            // Treasuries and rates
            ["30-day-federal-funds"] = Badge("FED", "#166534", "#dcfce7"),
            ["2-year-us-treasury-notes"] = Badge("2Y", "#065f46", "#d1fae5"),
            ["5-year-us-treasury-notes"] = Badge("5Y", "#047857", "#d1fae5"),
            ["10-year-us-treasury-notes"] = Badge("10Y", "#15803d", "#dcfce7"),

            // Energies
            ["crude-oil-light-sweet"] = Badge("OIL", "#111827", "#fef3c7"),
            ["gasoline-blendstock-rbob"] = Badge("RBOB", "#b45309", "#ffedd5"),
            ["heating-oil-ny-harbor-ulsd"] = Badge("HO", "#0369a1", "#e0f2fe"),
            ["NATURAL GAS"] = Badge("NG", "#047857", "#d1fae5"),

            // Grains
            ["corn"] = Badge("CORN", "#ca8a04", "#fef9c3"),
            ["soybeans"] = Badge("SOY", "#65a30d", "#ecfccb"),
            ["soybean-oil"] = Badge("OIL", "#84cc16", "#ecfccb"),
            ["soybean-meal"] = Badge("MEAL", "#4d7c0f", "#ecfccb"),
            ["wheat-srw"] = Badge("WHT", "#a16207", "#fef3c7"),
            ["wheat-hrw"] = Badge("HRW", "#92400e", "#ffedd5"),
            ["wheat-hrspring"] = Badge("HRS", "#b45309", "#fef3c7"),
            ["rough-rice"] = Badge("RICE", "#0f766e", "#ccfbf1"),
            ["canola"] = Badge("CAN", "#eab308", "#fef9c3"),

            // Livestock and dairy
            ["live-cattle"] = Badge("LC", "#7f1d1d", "#fee2e2"),
            ["feeder-cattle"] = Badge("FC", "#991b1b", "#fee2e2"),
            ["lean-hogs"] = Badge("LH", "#be185d", "#fce7f3"),
            ["milk-class-iii"] = Badge("MILK", "#2563eb", "#eff6ff"),
            ["non-fat-dry-milk"] = Badge("NFDM", "#0f766e", "#ccfbf1"),
            ["butter-cash-settled"] = Badge("BUT", "#ca8a04", "#fef9c3"),
            ["cheese-cash-settled"] = Badge("CHZ", "#d97706", "#ffedd5"),

            // Metals
            ["gold"] = Badge("XAU", "#b45309", "#fef3c7"),
            ["silver"] = Badge("XAG", "#475569", "#f1f5f9"),
            ["copper-grade-1"] = Badge("HG", "#c2410c", "#ffedd5"),
            ["palladium"] = Badge("XPD", "#334155", "#e2e8f0"),
            ["platinum"] = Badge("XPT", "#0f766e", "#ccfbf1"),
            ["aluminum-mwp"] = Badge("AL", "#64748b", "#f1f5f9"),

            // Softs
            ["cocoa"] = Badge("CC", "#78350f", "#fef3c7"),
            ["cotton-no-2"] = Badge("CT", "#2563eb", "#eff6ff"),
            ["coffee-c"] = Badge("KC", "#7c2d12", "#ffedd5"),
            ["sugar-no-11"] = Badge("SB", "#be123c", "#ffe4e6"),
            ["frzn-concentrated-orange-juice"] = Badge("OJ", "#ea580c", "#ffedd5"),
            ["random-length-lumber"] = Badge("LBR", "#854d0e", "#fef3c7")
        };

    [Parameter]
    public string? Slug { get; set; }

    [Parameter]
    public string? Name { get; set; }

    private InstrumentLogoAsset? Asset =>
        !string.IsNullOrWhiteSpace(Slug) && LogoAssets.TryGetValue(Slug, out var asset)
            ? asset
            : null;

    private string? LogoUrl => Asset?.Url;

    private string FallbackText => Asset?.FallbackText ?? BuildFallbackText();

    private string AltText => string.IsNullOrWhiteSpace(Name)
        ? $"{FallbackText} logo"
        : $"{Name} logo";

    private string GetAssetClass()
    {
        return Asset is null ? "td-cot-instrument-logo--fallback" : $"td-cot-instrument-logo--{Asset.Kind}";
    }

    private string BuildFallbackText()
    {
        var source = string.IsNullOrWhiteSpace(Name) ? Slug : Name;
        if (string.IsNullOrWhiteSpace(source))
        {
            return "?";
        }

        var parts = source
            .Split([' ', '-', '.', '_'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Where(part => !string.Equals(part, "and", StringComparison.OrdinalIgnoreCase))
            .ToArray();

        if (parts.Length == 0)
        {
            return source[..Math.Min(3, source.Length)].ToUpperInvariant();
        }

        return string.Concat(parts.Take(3).Select(part => char.ToUpperInvariant(part[0])));
    }

    private static InstrumentLogoAsset Badge(string label, string foreground, string background)
    {
        var svg = $"""
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 80 80">
              <rect width="80" height="80" rx="40" fill="{background}"/>
              <circle cx="40" cy="40" r="31" fill="#ffffff" opacity=".82"/>
              <text x="40" y="45" text-anchor="middle" dominant-baseline="middle" font-family="Arial, Helvetica, sans-serif" font-size="{GetBadgeFontSize(label)}" font-weight="800" fill="{foreground}">{label}</text>
            </svg>
            """;

        return new($"data:image/svg+xml,{Uri.EscapeDataString(svg)}", label, "symbol");
    }

    private static int GetBadgeFontSize(string label)
    {
        return label.Length switch
        {
            <= 2 => 28,
            3 => 24,
            _ => 19
        };
    }

    private sealed record InstrumentLogoAsset(string Url, string FallbackText, string Kind);
}
