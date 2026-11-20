using FluentValidation;
using MediatR;
using TradingDashboard.Application.Common.Notifications;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Repository.Auth;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Features.Auth.Commands.ResendActivationEmail;

public sealed class ResendActivationEmailCommandHandler(
    IAppUserQueryRepository userQueryRepository,
    IAuthNotificationService authNotificationService,
    IValidator<ResendActivationEmailCommand> validator)
    : IRequestHandler<ResendActivationEmailCommand, OperationalResult<AuthActionResponse>>
{
    private const string SuccessMessage = "If your account still needs activation, a new activation email has been sent.";

    public async Task<OperationalResult<AuthActionResponse>> Handle(ResendActivationEmailCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return OperationalResult<AuthActionResponse>.Fail(
                "Validation failed",
                "VALIDATION",
                validation.Errors.Select(x => x.ErrorMessage).Distinct().ToArray());
        }

        try
        {
            var user = await userQueryRepository.GetByIdentityAsync(request.Identity, cancellationToken);
            if (user is not null
                && user.Status == AppUserStatus.Active
                && !user.IsEmailVerified
                && user.SignupAppPlatform == SignupAppPlatform.Local)
            {
                await authNotificationService.SendActivationEmailAsync(user, cancellationToken);
            }

            return OperationalResult<AuthActionResponse>.Success(
                new AuthActionResponse(SuccessMessage),
                SuccessMessage);
        }
        catch (Exception ex)
        {
            return OperationalResult<AuthActionResponse>.Fail(
                "Unexpected error",
                "UNEXPECTED",
                ex.Message);
        }
    }
}
