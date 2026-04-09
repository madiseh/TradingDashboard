using Microsoft.AspNetCore.WebUtilities;

namespace TradingDashboard.Presentation.Blazor.Services;

public sealed class PortalNavigationService
{
    private const string GuestRole = "Guest";
    private const string AnalyzerRole = "Analyzer";
    private const string SuperAdminRole = "SuperAdmin";

    public bool CanAccessAdmin(string? role)
    {
        return HasRole(role, SuperAdminRole);
    }

    public bool CanAccessTradingPanel(string? role)
    {
        return IsKnownRole(role);
    }

    public string GetDefaultPortalRoute(string? role)
    {
        return CanAccessAdmin(role)
            ? RoutePaths.Ui.Admin
            : RoutePaths.Ui.TradingPanel;
    }

    public string ResolvePostLoginRoute(string? requestedReturnUrl, string? role)
    {
        var normalizedReturnUrl = NormalizeReturnUrl(requestedReturnUrl);
        if (!string.IsNullOrWhiteSpace(normalizedReturnUrl)
            && IsProtectedPortalRoute(normalizedReturnUrl)
            && CanAccessRoute(normalizedReturnUrl, role))
        {
            return normalizedReturnUrl;
        }

        return GetDefaultPortalRoute(role);
    }

    public string BuildLoginUrl(string? returnUrl = null, string? mode = null)
    {
        var query = new Dictionary<string, string?>();

        var normalizedReturnUrl = NormalizeReturnUrl(returnUrl);
        if (!string.IsNullOrWhiteSpace(normalizedReturnUrl))
        {
            query["returnUrl"] = normalizedReturnUrl;
        }

        if (!string.IsNullOrWhiteSpace(mode))
        {
            query["mode"] = mode.Trim().ToLowerInvariant();
        }

        return query.Count == 0
            ? RoutePaths.Ui.Login
            : QueryHelpers.AddQueryString(RoutePaths.Ui.Login, query);
    }

    private bool CanAccessRoute(string route, string? role)
    {
        if (route.StartsWith(RoutePaths.Ui.Admin, StringComparison.OrdinalIgnoreCase))
        {
            return CanAccessAdmin(role);
        }

        if (route.StartsWith(RoutePaths.Ui.TradingPanel, StringComparison.OrdinalIgnoreCase))
        {
            return CanAccessTradingPanel(role);
        }

        return false;
    }

    private static bool IsProtectedPortalRoute(string route)
    {
        return route.StartsWith(RoutePaths.Ui.Admin, StringComparison.OrdinalIgnoreCase)
            || route.StartsWith(RoutePaths.Ui.TradingPanel, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsKnownRole(string? role)
    {
        return HasRole(role, GuestRole)
            || HasRole(role, AnalyzerRole)
            || HasRole(role, SuperAdminRole);
    }

    private static bool HasRole(string? role, string expectedRole)
    {
        return string.Equals(role?.Trim(), expectedRole, StringComparison.OrdinalIgnoreCase);
    }

    private static string? NormalizeReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return null;
        }

        if (!Uri.TryCreate(returnUrl.Trim(), UriKind.Relative, out _))
        {
            return null;
        }

        var normalized = returnUrl.Trim();
        if (!normalized.StartsWith("/", StringComparison.Ordinal))
        {
            normalized = $"/{normalized.TrimStart('/')}";
        }

        if (normalized.StartsWith("//", StringComparison.Ordinal)
            || normalized.StartsWith("/login", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return normalized;
    }
}
