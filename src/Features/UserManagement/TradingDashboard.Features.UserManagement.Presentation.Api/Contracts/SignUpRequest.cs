namespace TradingDashboard.Presentation.Api.UserManagement.Contracts;

public sealed record SignUpRequest(string Username, string Email, string Password);
