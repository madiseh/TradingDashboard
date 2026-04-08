using FluentValidation;

namespace TradingDashboard.Application.Features.Subscriptions.Commands.PurchaseSubscription;

public sealed class PurchaseSubscriptionCommandValidator : AbstractValidator<PurchaseSubscriptionCommand>
{
    public PurchaseSubscriptionCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty();

        RuleFor(x => x.PlanCode)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(x => x.BillingCycle)
            .IsInEnum();
    }
}
