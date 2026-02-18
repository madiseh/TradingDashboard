namespace TradingDashboard.Presentation.Blazor.Contracts.Cot.Owned;

public enum CotReportModel
{
    CotLegacy,
    Cot
}

public static class CotReportModelExtensions
{
    public static string ToContractValue(this CotReportModel model)
    {
        return model switch
        {
            CotReportModel.CotLegacy => "cot-legacy",
            CotReportModel.Cot => "cot",
            _ => "cot"
        };
    }

    public static bool TryParse(string? value, out CotReportModel model)
    {
        if (string.Equals(value, "cot-legacy", StringComparison.OrdinalIgnoreCase))
        {
            model = CotReportModel.CotLegacy;
            return true;
        }

        if (string.Equals(value, "cot", StringComparison.OrdinalIgnoreCase))
        {
            model = CotReportModel.Cot;
            return true;
        }

        model = default;
        return false;
    }
}
