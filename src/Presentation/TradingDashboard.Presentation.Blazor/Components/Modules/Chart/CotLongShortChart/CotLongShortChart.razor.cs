using System.Globalization;

namespace TradingDashboard.Presentation.Blazor.Components.Modules.Chart;

public partial class CotLongShortChart
{
    private static readonly string[] LegacyCategoryOrder =
    [
        "non-commercial",
        "commercial",
        "nonreportable"
    ];

    private static readonly string[] StandardCategoryOrder =
    [
        "dealer",
        "asset-manager",
        "leveraged-money",
        "producer-merchant",
        "swap-dealers",
        "managed-money",
        "other-reportables",
        "nonreportable"
    ];

    [Parameter]
    public CotReportModel Model { get; set; } = CotReportModel.Cot;

    [Parameter]
    public CotLatestReportResponse? Report { get; set; }

    private IReadOnlyList<ChartBarViewModel> ChartItems => BuildChartItems();

    private IReadOnlyList<ChartBarViewModel> BuildChartItems()
    {
        if (Report?.Categories is null || Report.Categories.Count == 0)
        {
            return [];
        }

        IReadOnlyList<string> categoryOrder = Model == CotReportModel.CotLegacy
            ? LegacyCategoryOrder
            : StandardCategoryOrder;

        return categoryOrder
            .Select(GetCategoryByKey)
            .Where(static category => category is not null)
            .Select(static category => category!)
            .Select(ToChartBarViewModel)
            .Where(static item => item is not null)
            .Select(static item => item!)
            .ToList();
    }

    private CotLatestReportCategoryResponse? GetCategoryByKey(string key)
    {
        return Report?.Categories.FirstOrDefault(category =>
            string.Equals(category.Key, key, StringComparison.OrdinalIgnoreCase));
    }

    private static ChartBarViewModel? ToChartBarViewModel(CotLatestReportCategoryResponse category)
    {
        int longPositions = category.Long?.Positions ?? 0;
        int shortPositions = category.Short?.Positions ?? 0;
        int totalPositions = longPositions + shortPositions;

        if (totalPositions <= 0)
        {
            return null;
        }

        decimal longRatio = decimal.Round(longPositions * 100m / totalPositions, 1);
        decimal shortRatio = 100m - longRatio;

        return new ChartBarViewModel(
            category.Key,
            GetChartLabel(category),
            longPositions,
            shortPositions,
            longRatio,
            shortRatio);
    }

    private static string GetChartLabel(CotLatestReportCategoryResponse category)
    {
        return category.Key switch
        {
            "dealer" => "Dealer",
            "asset-manager" => "Asset Manager",
            "leveraged-money" => "Leveraged Funds",
            "producer-merchant" => "Producer/Merchant",
            "swap-dealers" => "Swap Dealers",
            "managed-money" => "Managed Money",
            "other-reportables" => "Other Reportables",
            "nonreportable" => "Non-Reportable",
            _ => category.Label
        };
    }

    private static string GetSegmentStyle(decimal ratio)
    {
        return $"height: {ratio.ToString("0.0", CultureInfo.InvariantCulture)}%;";
    }

    private static string BuildTooltip(ChartBarViewModel item)
    {
        return string.Create(
            CultureInfo.InvariantCulture,
            $"{item.Label} - Long: {item.LongPositions:N0} ({item.LongRatio:0.0}%), Short: {item.ShortPositions:N0} ({item.ShortRatio:0.0}%)");
    }
}
