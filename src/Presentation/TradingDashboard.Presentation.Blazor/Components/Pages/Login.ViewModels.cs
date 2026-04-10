using System.ComponentModel.DataAnnotations;

namespace TradingDashboard.Presentation.Blazor.Components.Pages;

public partial class Login
{
    private sealed class SignInFormModel
    {
        [Required(ErrorMessage = "Username or email is required.")]
        public string Identity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }

    private sealed class SignUpFormModel
    {
        [Required(ErrorMessage = "Username is required.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;
    }
}
