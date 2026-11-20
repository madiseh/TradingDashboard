using FluentValidation;
using MediatR;
using TradingDashboard.Application.Common.Notifications;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Repository.Auth;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandHandler(
    IAppUserQueryRepository userQueryRepository,
    IAuthNotificationService authNotificationService,
    IValidator<ForgotPasswordCommand> validator)
    : IRequestHandler<ForgotPasswordCommand, OperationalResult<AuthActionResponse>>
{
    private const string SuccessMessage = "If your account is eligible, a password reset email has been sent.";

    public async Task<OperationalResult<AuthActionResponse>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
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
            if (user is null || user.Status != AppUserStatus.Active)
            {
                return OperationalResult<AuthActionResponse>.Success(new AuthActionResponse(SuccessMessage), SuccessMessage);
            }

            if (!user.IsEmailVerified)
            {
                return OperationalResult<AuthActionResponse>.Fail(
                    "Email activation required",
                    "EMAIL_NOT_VERIFIED",
                    "Activate your email first, then request a password reset.");
            }

            await authNotificationService.SendPasswordResetEmailAsync(user, cancellationToken);

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
