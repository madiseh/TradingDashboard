using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Globalization;
using Microsoft.AspNetCore.WebUtilities;
using MudBlazor;
using TradingDashboard.Presentation.Blazor.Services;
using UserManagementContracts = TradingDashboard.Presentation.Blazor.Contracts.UserManagement.Generated;

namespace TradingDashboard.Presentation.Blazor.Components.Pages;

public partial class Login
{
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private IConfiguration Configuration { get; set; } = default!;

    [Inject]
    private AuthSession AuthSession { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private string _signInIdentity = string.Empty;
    private string _signInPassword = string.Empty;

    private string _signUpUsername = string.Empty;
    private string _signUpEmail = string.Empty;
    private string _signUpPassword = string.Empty;

    private bool _isSubmitting;
    private string? _errorMessage;
    private UserManagementContracts.AuthTokenResult? _authResult;
    private UserManagementContracts.MeResponse? _meResult;

    protected override async Task OnInitializedAsync()
    {
        await HandleGoogleSignInCallbackAsync();
    }

    private void BeginGoogleSignIn()
    {
        _errorMessage = null;

        var apiBaseUrl = Configuration[$"{HttpClientNames.ApiLogin}:BaseUrl"] ?? "https://localhost:7181";
        var returnUrl = NavigationManager.ToAbsoluteUri(NavigationManager.Uri).ToString();
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

            AuthSession.SetToken(authResult.AccessToken, authResult.ExpiresAtUtc);

            Snackbar.Add("Authentication succeeded.", Severity.Success);
            await LoadCurrentUserAsync();
            NavigateAfterAuthIfNeeded();
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
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AuthSession.AccessToken);

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

        var userId = query.TryGetValue("userId", out var userIdValue)
            ? userIdValue.ToString()
            : string.Empty;

        AuthSession.SetToken(accessToken, expiresAtUtc.ToUniversalTime());
        _authResult = new UserManagementContracts.AuthTokenResult
        {
            AccessToken = accessToken,
            ExpiresAtUtc = expiresAtUtc.ToUniversalTime(),
            UserId = userId,
            Username = username,
            Email = email
        };

        Snackbar.Add("Signed in with Google successfully.", Severity.Success);
        await LoadCurrentUserAsync();
        NavigateAfterAuthIfNeeded();
    }

    private void NavigateAfterAuthIfNeeded()
    {
        var redirectPath = GetSafeRedirectPath();
        if (string.IsNullOrWhiteSpace(redirectPath))
        {
            return;
        }

        NavigationManager.NavigateTo(redirectPath, forceLoad: true);
    }

    private string? GetSafeRedirectPath()
    {
        var currentUri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
        var query = QueryHelpers.ParseQuery(currentUri.Query);
        if (!query.TryGetValue("returnUrl", out var returnUrlValue))
        {
            return null;
        }

        var rawReturnUrl = returnUrlValue.ToString();
        if (string.IsNullOrWhiteSpace(rawReturnUrl))
        {
            return null;
        }

        if (rawReturnUrl.StartsWith("/", StringComparison.Ordinal))
        {
            return rawReturnUrl;
        }

        if (!Uri.TryCreate(rawReturnUrl, UriKind.Absolute, out var absoluteUri))
        {
            return null;
        }

        var appBaseUri = NavigationManager.ToAbsoluteUri(NavigationManager.BaseUri);
        var isSameHost =
            string.Equals(appBaseUri.Scheme, absoluteUri.Scheme, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(appBaseUri.Host, absoluteUri.Host, StringComparison.OrdinalIgnoreCase) &&
            appBaseUri.Port == absoluteUri.Port;

        return isSameHost ? absoluteUri.PathAndQuery : null;
    }
}
