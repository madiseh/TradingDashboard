namespace TradingDashboard.Presentation.Common.Routing;

public static class RoutePaths
{
    public static class Ui
    {
        public const string Home = "/";
        public const string Login = "/login";
        
        public const string Markets = "/markets";

        public const string CotHome = "/cot";
        public const string CotReports = $"{CotHome}/report";
    }
    public static class ApiCommon
    {
        public const string ApiVersionPrefix = "/api/v1";
        public const string Health = "/health";
    }
    public static class Api
    {
        public const string CommonBase = $"{ApiCommon.ApiVersionPrefix}/common";
        public const string MarketTypes = $"{CommonBase}/market-types";
        public const string Markets = $"{CommonBase}/markets";
        public const string MarketsDirectory = $"{CommonBase}/markets-directory";

        public static class Cot
        {
            public const string CotBase = $"{ApiCommon.ApiVersionPrefix}/cot";
            public const string CotInstruments = $"{CotBase}/instruments";
            public const string CotInstrumentBySlug = $"{CotInstruments}/{{instrumentSlug}}";
            public const string CotInstrumentLatestReport = $"{CotInstruments}/{{instrumentSlug}}/latest";
            public const string CotInstrumentAvailableDates = $"{CotInstruments}/{{instrumentSlug}}/available-dates";
            public const string CotInstrumentSeries = $"{CotInstruments}/{{instrumentSlug}}/series";
            public const string CotInstrumentsByGroup = $"{CotInstruments}/by-group";
        }
        public static class Login
        {
            public const string LoginBase = $"{ApiCommon.ApiVersionPrefix}/login";
            public const string SignUp = $"{LoginBase}/signup";
            public const string SignIn = $"{LoginBase}/signin";
            public const string Me = $"{LoginBase}/me";
        }
    }
}
