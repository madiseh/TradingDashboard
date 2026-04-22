using FluentValidation;

namespace TradingDashboard.Application.Features.Auth.Commands.ResendActivationEmail;

public sealed class ResendActivationEmailCommandValidator : AbstractValidator<ResendActivationEmailCommand>
{
    public ResendActivationEmailCommandValidator()
    {
        RuleFor(x => x.Identity)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(256);
    }
}
