using TradingDashboard.Domain.Entities;

namespace TradingDashboard.Application.Common.Notifications;

public interface IAuthNotificationService
{
    Task SendActivationEmailAsync(AppUser user, CancellationToken cancellationToken);
    Task SendPasswordResetEmailAsync(AppUser user, CancellationToken cancellationToken);
}
