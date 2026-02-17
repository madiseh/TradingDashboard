using System.Net.Http.Headers;
using System.Net.Http.Json;
using MudBlazor;
using TradingDashboard.Presentation.Blazor.Services;

namespace TradingDashboard.Presentation.Blazor.Components.Pages;

public partial class Login
{
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
    private AuthResultDto? _authResult;
    private MeResultDto? _meResult;

    private async Task SignInAsync()
    {
        await AuthenticateAsync(
            RoutePaths.Api.Login.SignIn,
            new SignInRequestDto(_signInIdentity, _signInPassword));
    }

    private async Task SignUpAsync()
    {
        await AuthenticateAsync(
            RoutePaths.Api.Login.SignUp,
            new SignUpRequestDto(_signUpUsername, _signUpEmail, _signUpPassword));
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

            var authResult = await response.Content.ReadFromJsonAsync<AuthResultDto>();
            if (authResult is null)
            {
                _errorMessage = "Authentication response is empty.";
                Snackbar.Add(_errorMessage, Severity.Error);
                _authResult = null;
                AuthSession.Clear();
                return;
            }

            _authResult = authResult;
            AuthSession.SetToken(authResult.AccessToken, authResult.ExpiresAtUtc);

            Snackbar.Add("Authentication succeeded.", Severity.Success);
            await LoadCurrentUserAsync();
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

        _meResult = await response.Content.ReadFromJsonAsync<MeResultDto>();
    }

    private static async Task<string> ExtractErrorAsync(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadFromJsonAsync<ErrorResponseDto>();
        if (payload is null)
        {
            return $"Request failed with status {(int)response.StatusCode}.";
        }

        return payload.Errors is { Count: > 0 }
            ? string.Join(" | ", payload.Errors)
            : payload.Message ?? $"Request failed with status {(int)response.StatusCode}.";
    }

    private sealed record SignInRequestDto(string Identity, string Password);

    private sealed record SignUpRequestDto(string Username, string Email, string Password);

    private sealed record AuthResultDto(
        string AccessToken,
        DateTime ExpiresAtUtc,
        Guid UserId,
        string Username,
        string Email);

    private sealed record MeResultDto(string? UserId, string? Username, string? Email);

    private sealed record ErrorResponseDto(string? Message, string? ErrorCode, IReadOnlyList<string>? Errors);
}
