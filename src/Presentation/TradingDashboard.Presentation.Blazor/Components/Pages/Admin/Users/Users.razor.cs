using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Localization;
using MudBlazor;
using TradingDashboard.Presentation.Blazor.Services;
using TradingDashboard.Presentation.Common.Resources.Components.Pages.Admin.Users;
using LoginContracts = TradingDashboard.Presentation.Blazor.Contracts.Login.Generated;

namespace TradingDashboard.Presentation.Blazor.Components.Pages.Admin.Users;

public partial class Users
{
    private bool _isLoading;
    private bool _isForbidden;
    private bool _isSavingPassword;
    private string _searchText = string.Empty;
    private string? _errorMessage;
    private string? _selectedPasswordUserId;
    private string _newPassword = string.Empty;
    private string _confirmPassword = string.Empty;
    private LoginContracts.GetAdminUsersResponse? _adminUsers;

    [Inject]
    private IStringLocalizer<UsersResource> L { get; set; } = default!;

    [Inject]
    private AuthSession AuthSession { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        if (AuthSession.IsAuthenticated)
        {
            await LoadUsersAsync();
        }
    }

    private async Task HandleSearchAsync()
    {
        await LoadUsersAsync();
    }

    private async Task ResetSearchAsync()
    {
        _searchText = string.Empty;
        await LoadUsersAsync();
    }

    private async Task RefreshAsync()
    {
        await LoadUsersAsync();
    }

    private Task OnSearchTextChanged(string value)
    {
        _searchText = value;
        return Task.CompletedTask;
    }

    private async Task LoadUsersAsync()
    {
        if (!AuthSession.IsAuthenticated)
        {
            _adminUsers = null;
            _isForbidden = false;
            _errorMessage = null;
            return;
        }

        _isLoading = true;
        _isForbidden = false;
        _errorMessage = null;

        try
        {
            var client = HttpClientFactory.CreateClient(HttpClientNames.ApiLogin);
            AuthSession.Apply(client);

            var route = string.IsNullOrWhiteSpace(_searchText)
                ? RoutePaths.Api.Login.AdminUsers
                : $"{RoutePaths.Api.Login.AdminUsers}?search={Uri.EscapeDataString(_searchText.Trim())}";

            var response = await client.GetAsync(route);

            if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                _adminUsers = null;
                _isForbidden = true;
                return;
            }

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                AuthSession.Clear();
                _adminUsers = null;
                _errorMessage = L["States.AuthRequiredDescription"];
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                _adminUsers = null;
                _errorMessage = await ExtractErrorAsync(response);
                return;
            }

            _adminUsers = await response.Content.ReadFromJsonAsync<LoginContracts.GetAdminUsersResponse>();
            if (_adminUsers is null)
            {
                _errorMessage = L["Errors.EmptyResponse"];
                return;
            }

            if (_adminUsers.Users?.Any(user => string.Equals(user.UserId, _selectedPasswordUserId, StringComparison.OrdinalIgnoreCase)) != true)
            {
                ClosePasswordEditor();
            }
        }
        catch (Exception ex)
        {
            _adminUsers = null;
            _errorMessage = L["Errors.LoadUsersWithMessage", ex.Message];
        }
        finally
        {
            _isLoading = false;
        }
    }

    private void TogglePasswordEditor(LoginContracts.AdminUserItemResponse user)
    {
        if (IsPasswordEditorOpen(user))
        {
            ClosePasswordEditor();
            return;
        }

        _selectedPasswordUserId = user.UserId;
        _newPassword = string.Empty;
        _confirmPassword = string.Empty;
    }

    private void ClosePasswordEditor()
    {
        _selectedPasswordUserId = null;
        _newPassword = string.Empty;
        _confirmPassword = string.Empty;
    }

    private bool IsPasswordEditorOpen(LoginContracts.AdminUserItemResponse user)
    {
        return string.Equals(user.UserId, _selectedPasswordUserId, StringComparison.OrdinalIgnoreCase);
    }

    private async Task ChangePasswordAsync(LoginContracts.AdminUserItemResponse user)
    {
        if (string.IsNullOrWhiteSpace(_newPassword))
        {
            Snackbar.Add(L["Messages.PasswordRequired"], Severity.Warning);
            return;
        }

        if (!string.Equals(_newPassword, _confirmPassword, StringComparison.Ordinal))
        {
            Snackbar.Add(L["Messages.PasswordMismatch"], Severity.Warning);
            return;
        }

        _isSavingPassword = true;

        try
        {
            var client = HttpClientFactory.CreateClient(HttpClientNames.ApiLogin);
            AuthSession.Apply(client);

            var response = await client.PostAsJsonAsync(
                $"{RoutePaths.Api.Login.AdminUsers}/{user.UserId}/change-password",
                new LoginContracts.ChangeUserPasswordRequest
                {
                    NewPassword = _newPassword
                });

            if (!response.IsSuccessStatusCode)
            {
                Snackbar.Add(await ExtractErrorAsync(response), Severity.Error);
                return;
            }

            var payload = await response.Content.ReadFromJsonAsync<LoginContracts.ChangeUserPasswordResponse>();
            Snackbar.Add(payload?.Message ?? L["Messages.PasswordChanged"], Severity.Success);
            ClosePasswordEditor();
        }
        catch (Exception ex)
        {
            Snackbar.Add(L["Errors.ChangePasswordWithMessage", ex.Message], Severity.Error);
        }
        finally
        {
            _isSavingPassword = false;
        }
    }

    private IEnumerable<AdminNavigationItemViewModel> GetNavigationItems()
    {
        return
        [
            new AdminNavigationItemViewModel(L["Nav.Dashboard"], "bi bi-grid-1x2-fill", RoutePaths.Ui.AdminUsers, false, false),
            new AdminNavigationItemViewModel(L["Nav.Users"], "bi bi-people-fill", RoutePaths.Ui.AdminUsers, true, true),
            new AdminNavigationItemViewModel(L["Nav.AccessControl"], "bi bi-shield-lock-fill", RoutePaths.Ui.AdminUsers, false, false),
            new AdminNavigationItemViewModel(L["Nav.Analytics"], "bi bi-bar-chart-fill", RoutePaths.Ui.AdminUsers, false, false)
        ];
    }

    private IEnumerable<AdminSummaryCardViewModel> GetSummaryCards(LoginContracts.GetAdminUsersResponse response)
    {
        return
        [
            new AdminSummaryCardViewModel(L["Summary.TotalUsers"], response.TotalUsers.ToString(), L["Summary.TotalUsersDescription"]),
            new AdminSummaryCardViewModel(L["Summary.ActiveUsers"], response.ActiveUsers.ToString(), L["Summary.ActiveUsersDescription"]),
            new AdminSummaryCardViewModel(L["Summary.DisabledUsers"], response.DisabledUsers.ToString(), L["Summary.DisabledUsersDescription"]),
            new AdminSummaryCardViewModel(L["Summary.SuperAdmins"], response.SuperAdminUsers.ToString(), L["Summary.SuperAdminsDescription"])
        ];
    }

    private string GetNavigationItemClass(AdminNavigationItemViewModel item)
    {
        return item.IsActive
            ? "admin-users-page__nav-item admin-users-page__nav-item--active"
            : "admin-users-page__nav-item";
    }

    private static string GetNavigationHref(AdminNavigationItemViewModel item)
    {
        return item.IsAvailable
            ? item.Href
            : "javascript:void(0)";
    }

    private string GetRoleLabel(string? role)
    {
        return role switch
        {
            "SuperAdmin" => L["Role.SuperAdmin"],
            "Analyzer" => L["Role.Analyzer"],
            _ => L["Role.Guest"]
        };
    }

    private string GetStatusLabel(string? status)
    {
        return string.Equals(status, "Disabled", StringComparison.OrdinalIgnoreCase)
            ? L["Status.Disabled"]
            : L["Status.Active"];
    }

    private static Color GetRoleColor(string? role)
    {
        return role switch
        {
            "SuperAdmin" => Color.Primary,
            "Analyzer" => Color.Info,
            _ => Color.Default
        };
    }

    private static Color GetStatusColor(string? status)
    {
        return string.Equals(status, "Disabled", StringComparison.OrdinalIgnoreCase)
            ? Color.Error
            : Color.Success;
    }

    private string FormatDate(DateTime? value)
    {
        return value.HasValue
            ? value.Value.ToLocalTime().ToString("yyyy-MM-dd HH:mm")
            : L["Common.Never"];
    }

    private static string GetInitials(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "U";
        }

        return new string(value
            .Split([' ', '.', '_', '-'], StringSplitOptions.RemoveEmptyEntries)
            .Select(part => char.ToUpperInvariant(part[0]))
            .Take(2)
            .ToArray());
    }

    private async Task<string> ExtractErrorAsync(HttpResponseMessage response)
    {
        var payload = await response.Content.ReadFromJsonAsync<LoginContracts.ApiErrorResponse>();
        if (payload is null)
        {
            return L["Errors.RequestFailedWithStatus", (int)response.StatusCode];
        }

        return payload.Errors is { Count: > 0 }
            ? string.Join(" | ", payload.Errors)
            : payload.Message ?? L["Errors.RequestFailedWithStatus", (int)response.StatusCode];
    }
}
