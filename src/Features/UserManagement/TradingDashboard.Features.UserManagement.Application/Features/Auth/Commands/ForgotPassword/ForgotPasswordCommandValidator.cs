using FluentValidation;

namespace TradingDashboard.Application.Features.Auth.Commands.ForgotPassword;

public sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Identity)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(256);
    }
}
