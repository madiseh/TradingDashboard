using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
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
    private AuthSession AuthSession { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private PortalNavigationService PortalNavigation { get; set; } = default!;

    private readonly SignInFormModel _signInModel = new();
    private readonly SignUpFormModel _signUpModel = new();
    private MudForm? _signInForm;
    private MudForm? _signUpForm;
    private int _activeTabIndex;
    private bool _isSubmitting;
    private string? _errorMessage;
    private UserManagementContracts.AuthTokenResult? _authResult;
    private UserManagementContracts.MeResponse? _meResult;

    private bool IsSignInDisabled =>
        _isSubmitting
        || string.IsNullOrWhiteSpace(_signInModel.Identity)
        || string.IsNullOrWhiteSpace(_signInModel.Password);

    private bool IsSignUpDisabled =>
        _isSubmitting
        || string.IsNullOrWhiteSpace(_signUpModel.Username)
        || string.IsNullOrWhiteSpace(_signUpModel.Email)
        || string.IsNullOrWhiteSpace(_signUpModel.Password);

    protected override void OnParametersSet()
    {
        _activeTabIndex = string.Equals(Mode, "signup", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
    }

    private async Task SignInAsync()
    {
        if (_signInForm is not null)
        {
            await _signInForm.Validate();
            if (!_signInForm.IsValid)
            {
                return;
            }
        }

        await AuthenticateAsync(
            RoutePaths.Api.Login.SignIn,
            new UserManagementContracts.SignInRequest
            {
                Identity = _signInModel.Identity.Trim(),
                Password = _signInModel.Password
            });
    }

    private async Task SignUpAsync()
    {
        if (_signUpForm is not null)
        {
            await _signUpForm.Validate();
            if (!_signUpForm.IsValid)
            {
                return;
            }
        }

        await AuthenticateAsync(
            RoutePaths.Api.Login.SignUp,
            new UserManagementContracts.SignUpRequest
            {
                Username = _signUpModel.Username.Trim(),
                Email = _signUpModel.Email.Trim(),
                Password = _signUpModel.Password
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

    private void NavigateToAuthorizedPortal()
    {
        var role = _meResult?.Role ?? _authResult?.Role ?? AuthSession.Role;
        var targetRoute = PortalNavigation.ResolvePostLoginRoute(ReturnUrl, role);
        NavigationManager.NavigateTo(targetRoute);
    }
}
