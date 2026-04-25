namespace TradingDashboard.Presentation.Common.Constants;

public static class SiteBranding
{
    public const string BrandName = "TerminalPrime";
    public const string Domain = "terminalprime.com";
    public const string BaseUrl = "https://terminalprime.com";
    public const string SupportEmail = "support@terminalprime.com";
    public const string GoogleSiteVerificationToken = "Fzq4bY3rBfibL1NWV6gns2WnnRGOHPlY6gi2SgiHAbU";
    public const int CopyrightYear = 2026;

    public const string AssetBasePath = "/images/terminalprime";
    public const string ThemeAssetBasePath = "/terminalprime/assets";

    public static string Image(string relativePath)
    {
        return $"{AssetBasePath}/{relativePath.TrimStart('/')}";
    }

    public static string ThemeAsset(string relativePath)
    {
        return $"{ThemeAssetBasePath}/{relativePath.TrimStart('/')}";
    }

    public static string AbsoluteUrl(string path)
    {
        if (Uri.TryCreate(path, UriKind.Absolute, out var absoluteUri))
        {
            return absoluteUri.ToString();
        }

        var normalizedPath = path.StartsWith("/", StringComparison.Ordinal) ? path : $"/{path}";
        return $"{BaseUrl}{normalizedPath}";
    }
}
