using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using TradingDashboard.Presentation.Blazor.Services;
using TradingDashboard.Presentation.Common.Resources.Components.Pages.Workspace.Login;
using UserManagementContracts = TradingDashboard.Presentation.Blazor.Contracts.UserManagement.Generated;

namespace TradingDashboard.Presentation.Blazor.Components.Pages.Workspace.Login;

public partial class Login
{
    private const string EmailNotVerifiedErrorCode = "EMAIL_NOT_VERIFIED";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

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

    [Inject]
    private IStringLocalizer<LoginResource> L { get; set; } = default!;

    private readonly SignInFormModel _signInModel = new();
    private readonly SignUpFormModel _signUpModel = new();
    private MudForm? _signInForm;
    private MudForm? _signUpForm;
    private int _activeTabIndex;
    private bool _isSubmitting;
    private string? _errorMessage;
    private UserManagementContracts.AuthTokenResult? _authResult;
    private UserManagementContracts.MeResponse? _meResult;
    private bool _canSendActivationEmail;

    private bool IsSignInDisabled =>
        _isSubmitting
        || string.IsNullOrWhiteSpace(_signInModel.Identity)
        || string.IsNullOrWhiteSpace(_signInModel.Password);

    private bool IsSignUpDisabled =>
        _isSubmitting
        || string.IsNullOrWhiteSpace(_signUpModel.Username)
        || string.IsNullOrWhiteSpace(_signUpModel.Email)
        || string.IsNullOrWhiteSpace(_signUpModel.Password);

    private bool IsIdentityActionDisabled =>
        _isSubmitting || string.IsNullOrWhiteSpace(_signInModel.Identity);

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
        _canSendActivationEmail = false;

        try
        {
            var client = HttpClientFactory.CreateClient(HttpClientNames.ApiLogin);
            var response = await client.PostAsJsonAsync(route, payload);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = ExtractError(response, content, out var errorCode);
                _canSendActivationEmail = string.Equals(errorCode, EmailNotVerifiedErrorCode, StringComparison.OrdinalIgnoreCase);
                Snackbar.Add(_errorMessage, Severity.Error);
                _authResult = null;
                AuthSession.Clear();
                return;
            }

            var authResult = DeserializeJson<UserManagementContracts.AuthTokenResult>(content);
            if (authResult is null)
            {
                _errorMessage = BuildInvalidSuccessPayloadMessage(response, content);
                Snackbar.Add(_errorMessage, Severity.Error);
                _authResult = null;
                AuthSession.Clear();
                return;
            }

            _authResult = authResult;
            if (string.IsNullOrWhiteSpace(authResult.AccessToken))
            {
                _errorMessage = L["Errors.MissingAccessToken"].Value;
                Snackbar.Add(_errorMessage, Severity.Error);
                _authResult = null;
                AuthSession.Clear();
                return;
            }

            AuthSession.SetAuthenticatedUser(authResult);

            Snackbar.Add(L["Messages.AuthenticationSucceeded"].Value, Severity.Success);
            await LoadCurrentUserAsync();
            NavigateToAuthorizedPortal();
        }
        catch (Exception ex)
        {
            _errorMessage = L["Errors.AuthenticationFailedWithMessage", ex.Message].Value;
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

    private async Task ForgotPasswordAsync()
    {
        await SubmitIdentityActionAsync(
            RoutePaths.Api.Login.ForgotPassword,
            new UserManagementContracts.ForgotPasswordRequest
            {
                Identity = GetTrimmedIdentity()
            },
            L["Messages.ForgotPasswordSubmitted"].Value);
    }

    private async Task SendActivationEmailAsync()
    {
        var succeeded = await SubmitIdentityActionAsync(
            RoutePaths.Api.Login.ResendActivationEmail,
            new UserManagementContracts.ResendActivationEmailRequest
            {
                Identity = GetTrimmedIdentity()
            },
            L["Messages.ActivationEmailSubmitted"].Value);

        if (succeeded)
        {
            _canSendActivationEmail = false;
        }
    }

    private async Task<bool> SubmitIdentityActionAsync(string route, object payload, string fallbackSuccessMessage)
    {
        if (string.IsNullOrWhiteSpace(GetTrimmedIdentity()))
        {
            Snackbar.Add(L["Validation.IdentityRequiredForAction"].Value, Severity.Warning);
            return false;
        }

        _activeTabIndex = 0;
        _isSubmitting = true;

        try
        {
            var client = HttpClientFactory.CreateClient(HttpClientNames.ApiLogin);
            var response = await client.PostAsJsonAsync(route, payload);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = ExtractError(response, content, out var errorCode);
                _canSendActivationEmail = string.Equals(errorCode, EmailNotVerifiedErrorCode, StringComparison.OrdinalIgnoreCase);
                Snackbar.Add(_errorMessage, Severity.Error);
                return false;
            }

            _errorMessage = null;
            var actionResponse = DeserializeJson<UserManagementContracts.AuthActionResponse>(content);
            Snackbar.Add(actionResponse?.Message ?? fallbackSuccessMessage, Severity.Success);
            return true;
        }
        catch (Exception ex)
        {
            _errorMessage = L["Errors.ActionFailedWithMessage", ex.Message].Value;
            Snackbar.Add(_errorMessage, Severity.Error);
            return false;
        }
        finally
        {
            _isSubmitting = false;
        }
    }

    private string ExtractError(HttpResponseMessage response, string content, out string? errorCode)
    {
        var payload = DeserializeJson<UserManagementContracts.ApiErrorResponse>(content);
        if (payload is null)
        {
            errorCode = null;
            return L["Errors.RequestFailedWithStatus", (int)response.StatusCode].Value;
        }

        errorCode = payload.ErrorCode;

        return payload.Errors is { Count: > 0 }
            ? string.Join(" | ", payload.Errors)
            : payload.Message ?? L["Errors.RequestFailedWithStatus", (int)response.StatusCode].Value;
    }

    private static T? DeserializeJson<T>(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return default;
        }

        try
        {
            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }
        catch (JsonException)
        {
            return default;
        }
    }

    private string BuildInvalidSuccessPayloadMessage(HttpResponseMessage response, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return L["Errors.EmptySuccessPayloadWithStatus", (int)response.StatusCode].Value;
        }

        var mediaType = response.Content.Headers.ContentType?.MediaType;
        return string.IsNullOrWhiteSpace(mediaType)
            ? L["Errors.InvalidSuccessPayload"].Value
            : L["Errors.InvalidSuccessPayloadWithMediaType", mediaType].Value;
    }

    private void NavigateToAuthorizedPortal()
    {
        var role = _meResult?.Role ?? _authResult?.Role ?? AuthSession.Role;
        var targetRoute = PortalNavigation.ResolvePostLoginRoute(ReturnUrl, role);
        NavigationManager.NavigateTo(targetRoute);
    }

    private string GetTrimmedIdentity()
    {
        return _signInModel.Identity.Trim();
    }
}
