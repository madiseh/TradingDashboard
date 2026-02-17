namespace TradingDashboard.Presentation.Blazor.Contracts.Generated.Cot;

public enum CotReportModel
{
    CotLegacy = 1,
    Cot = 2
}

public static class CotReportModelExtensions
{
    public static string ToContractValue(this CotReportModel model)
    {
        return model switch
        {
            CotReportModel.CotLegacy => nameof(CotReportModel.CotLegacy),
            CotReportModel.Cot => nameof(CotReportModel.Cot),
            _ => nameof(CotReportModel.Cot)
        };
    }

    public static bool TryParse(string? value, out CotReportModel model)
    {
        if (Enum.TryParse(value, true, out CotReportModel parsed) &&
            Enum.IsDefined(parsed))
        {
            model = parsed;
            return true;
        }

        model = CotReportModel.Cot;
        return false;
    }
}
