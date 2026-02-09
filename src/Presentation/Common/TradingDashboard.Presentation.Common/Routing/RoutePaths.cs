namespace TradingDashboard.Presentation.Common.Routing;

public static class RoutePaths
{
    public static class Ui
    {
        public const string Home = "/";
        public const string CotReports = Home;
        public const string Markets = "/markets";
        public const string Cot = "/cot";
    }

    public static class Api
    {
        public const string Health = "/health";
        public const string ApiVersionPrefix = "/api/v1/";
        public const string CommonBase = $"{ApiVersionPrefix}common";
        public const string CotBase = $"{ApiVersionPrefix}cot";
        public const string MarketTypes = $"{CommonBase}/market-types";
        public const string Markets = $"{CommonBase}/markets";
        public const string MarketsDirectory = $"{CommonBase}/markets-directory";
        public const string CotInstrumentBySlug = $"{CotBase}/instruments/{{instrumentSlug}}";
    }
}
