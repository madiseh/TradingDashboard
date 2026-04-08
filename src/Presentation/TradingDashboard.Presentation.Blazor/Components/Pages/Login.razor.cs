using System.Net.Http.Json;
using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using TradingDashboard.Presentation.Blazor.Services;
using UserManagementContracts = TradingDashboard.Presentation.Blazor.Contracts.UserManagement.Generated;

namespace TradingDashboard.Presentation.Blazor.Components.Pages;

public partial class Login
{
    [SupplyParameterFromQuery(Name = "mode")]
    public string? Mode { get; set; }

    [SupplyParameterFromQuery(Name = "returnUrl")]
    public string? ReturnUrl { get; set; }

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IConfiguration Configuration { get; set; } = default!;

    [Inject]
    private AuthSession AuthSession { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private PortalNavigationService PortalNavigation { get; set; } = default!;

    private string _signInIdentity = string.Empty;
    private string _signInPassword = string.Empty;

    private string _signUpUsername = string.Empty;
    private string _signUpEmail = string.Empty;
    private string _signUpPassword = string.Empty;

    private int _activeTabIndex;
    private bool _isSubmitting;
    private string? _errorMessage;
    private LoginContracts.AuthTokenResult? _authResult;
    private LoginContracts.MeResponse? _meResult;
    private string? _lastProcessedQuery;

    private IReadOnlyList<PortalDestinationCard> PortalDestinations =>
    [
        new("SuperAdmin", "Full isolated admin environment with left navigation and dashboard controls.", RoutePaths.Ui.Admin),
        new("Analyzer", "Trading workspace access without admin entry.", RoutePaths.Ui.TradingPanel),
        new("Guest", "Default signup role with Trading Panel access.", RoutePaths.Ui.TradingPanel)
    ];

    protected override async Task OnParametersSetAsync()
    {
        _activeTabIndex = string.Equals(Mode, "signup", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        await HandleGoogleSignInCallbackAsync();
    }

    private void BeginGoogleSignIn()
    {
        _errorMessage = null;

        var apiBaseUrl = Configuration[$"{HttpClientNames.ApiLogin}:BaseUrl"] ?? "https://localhost:7181";
        var currentUri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var returnUrl = BuildGoogleCallbackUrl(currentUri);
        var startUrl = QueryHelpers.AddQueryString(
            $"{apiBaseUrl.TrimEnd('/')}{RoutePaths.Api.Login.GoogleStart}",
            "returnUrl",
            returnUrl);

        NavigationManager.NavigateTo(startUrl, forceLoad: true);
    }

    private async Task SignInAsync()
    {
        await AuthenticateAsync(
            RoutePaths.Api.Login.SignIn,
            new UserManagementContracts.SignInRequest
            {
                Identity = _signInIdentity,
                Password = _signInPassword
            });
    }

    private async Task SignUpAsync()
    {
        await AuthenticateAsync(
            RoutePaths.Api.Login.SignUp,
            new UserManagementContracts.SignUpRequest
            {
                Username = _signUpUsername,
                Email = _signUpEmail,
                Password = _signUpPassword
            });
    }

    private async Task AuthenticateAsync(string route, object payload)
    {
        _isSubmitting = true;
        _errorMessage = null;
        _meResult = null;

        try
        {
            var client = HttpClientFactory.CreateClient(HttpClientNames.ApiLogin);
            var response = await client.PostAsJsonAsync(route, payload);

            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = await ExtractErrorAsync(response);
                Snackbar.Add(_errorMessage, Severity.Error);
                _authResult = null;
                AuthSession.Clear();
                return;
            }

            var authResult = await response.Content.ReadFromJsonAsync<UserManagementContracts.AuthTokenResult>();
            if (authResult is null)
            {
                _errorMessage = "Authentication response is empty.";
                Snackbar.Add(_errorMessage, Severity.Error);
                _authResult = null;
                AuthSession.Clear();
                return;
            }

            _authResult = authResult;
            if (string.IsNullOrWhiteSpace(authResult.AccessToken))
            {
                _errorMessage = "Authentication response does not contain access token.";
                Snackbar.Add(_errorMessage, Severity.Error);
                _authResult = null;
                AuthSession.Clear();
                return;
            }

            AuthSession.SetAuthenticatedUser(authResult);

            Snackbar.Add("Authentication succeeded.", Severity.Success);
            await LoadCurrentUserAsync();
            NavigateToAuthorizedPortal();
        }
        catch (Exception ex)
        {
            _errorMessage = $"Authentication request failed. {ex.Message}";
            Snackbar.Add(_errorMessage, Severity.Error);
            _authResult = null;
            AuthSession.Clear();
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private async Task LoadCurrentUserAsync()
    {
        if (!AuthSession.IsAuthenticated)
        {
            _meResult = null;
            return;
        }

        var client = HttpClientFactory.CreateClient(HttpClientNames.ApiLogin);
        AuthSession.Apply(client);

        var response = await client.GetAsync(RoutePaths.Api.Login.Me);
        if (!response.IsSuccessStatusCode)
        {
            _meResult = null;
            return;
        }

        _meResult = await response.Content.ReadFromJsonAsync<UserManagementContracts.MeResponse>();
    }

    private static async Task<string> ExtractErrorAsync(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadFromJsonAsync<UserManagementContracts.ApiErrorResponse>();
        if (payload is null)
        {
            return $"Request failed with status {(int)response.StatusCode}.";
        }

        return payload.Errors is { Count: > 0 }
            ? string.Join(" | ", payload.Errors)
            : payload.Message ?? $"Request failed with status {(int)response.StatusCode}.";
    }

    private async Task HandleGoogleSignInCallbackAsync()
    {
        var currentUri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        if (string.Equals(_lastProcessedQuery, currentUri.Query, StringComparison.Ordinal))
        {
            return;
        }

        _lastProcessedQuery = currentUri.Query;
        var query = QueryHelpers.ParseQuery(currentUri.Query);

        if (query.TryGetValue("authError", out var errorValue))
        {
            _errorMessage = errorValue.ToString();
            Snackbar.Add(_errorMessage, Severity.Error);
            return;
        }

        if (!query.TryGetValue("accessToken", out var accessTokenValue))
        {
            return;
        }

        var accessToken = accessTokenValue.ToString();
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            _errorMessage = "Google authentication returned an empty token.";
            Snackbar.Add(_errorMessage, Severity.Error);
            return;
        }

        if (!query.TryGetValue("expiresAtUtc", out var expiresAtValue)
            || !DateTime.TryParse(
                expiresAtValue.ToString(),
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out var expiresAtUtc))
        {
            _errorMessage = "Google authentication returned an invalid expiration timestamp.";
            Snackbar.Add(_errorMessage, Severity.Error);
            return;
        }

        var username = query.TryGetValue("username", out var usernameValue)
            ? usernameValue.ToString()
            : string.Empty;

        var email = query.TryGetValue("email", out var emailValue)
            ? emailValue.ToString()
            : string.Empty;

        var role = query.TryGetValue("role", out var roleValue)
            ? roleValue.ToString()
            : string.Empty;

        var userId = query.TryGetValue("userId", out var userIdValue)
            ? userIdValue.ToString()
            : string.Empty;

        _authResult = new LoginContracts.AuthTokenResult
        {
            AccessToken = accessToken,
            ExpiresAtUtc = expiresAtUtc.ToUniversalTime(),
            UserId = userId,
            Username = username,
            Email = email,
            Role = role
        };
        AuthSession.SetAuthenticatedUser(_authResult);

        Snackbar.Add("Signed in with Google successfully.", Severity.Success);
        await LoadCurrentUserAsync();
        NavigateToAuthorizedPortal();
    }

    private void NavigateToAuthorizedPortal()
    {
        var role = _meResult?.Role ?? _authResult?.Role ?? AuthSession.Role;
        var targetRoute = PortalNavigation.ResolvePostLoginRoute(ReturnUrl, role);
        NavigationManager.NavigateTo(targetRoute);
    }

    private static string BuildGoogleCallbackUrl(Uri currentUri)
    {
        var callbackBase = currentUri.GetLeftPart(UriPartial.Path);
        var currentQuery = QueryHelpers.ParseQuery(currentUri.Query);
        var preservedQuery = new Dictionary<string, string?>();

        if (currentQuery.TryGetValue("mode", out var modeValue))
        {
            preservedQuery["mode"] = modeValue.ToString();
        }

        if (currentQuery.TryGetValue("returnUrl", out var returnUrlValue))
        {
            preservedQuery["returnUrl"] = returnUrlValue.ToString();
        }

        return preservedQuery.Count == 0
            ? callbackBase
            : QueryHelpers.AddQueryString(callbackBase, preservedQuery);
    }
}
