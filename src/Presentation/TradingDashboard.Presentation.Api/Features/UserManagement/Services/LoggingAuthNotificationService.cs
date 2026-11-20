using Microsoft.Extensions.Logging;
using TradingDashboard.Application.Common.Notifications;
using TradingDashboard.Domain.Entities;

namespace TradingDashboard.Presentation.Api.UserManagement.Services;

public sealed class LoggingAuthNotificationService(ILogger<LoggingAuthNotificationService> logger)
    : IAuthNotificationService
{
    public Task SendActivationEmailAsync(AppUser user, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Activation email requested for user {UserId} ({Email}).",
            user.Id,
            user.Email.Value);

        return Task.CompletedTask;
    }

    public Task SendPasswordResetEmailAsync(AppUser user, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Password reset email requested for user {UserId} ({Email}).",
            user.Id,
            user.Email.Value);

        return Task.CompletedTask;
    }
}
