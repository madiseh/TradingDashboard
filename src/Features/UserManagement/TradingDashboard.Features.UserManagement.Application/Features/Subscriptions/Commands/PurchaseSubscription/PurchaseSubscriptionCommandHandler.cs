using FluentValidation;
using MediatR;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Common.Subscriptions;
using TradingDashboard.Application.Repository.Auth;
using TradingDashboard.Application.Repository.Subscriptions;
using TradingDashboard.Domain.Entities;

namespace TradingDashboard.Application.Features.Subscriptions.Commands.PurchaseSubscription;

public sealed class PurchaseSubscriptionCommandHandler(
    IAppUserQueryRepository appUserQueryRepository,
    ISubscriptionQueryRepository subscriptionQueryRepository,
    ISubscriptionCommandRepository subscriptionCommandRepository,
    IValidator<PurchaseSubscriptionCommand> validator)
    : IRequestHandler<PurchaseSubscriptionCommand, OperationalResult<SubscriptionSummaryResult>>
{
    public async Task<OperationalResult<SubscriptionSummaryResult>> Handle(
        PurchaseSubscriptionCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return OperationalResult<SubscriptionSummaryResult>.Fail(
                "Validation failed",
                "VALIDATION",
                validation.Errors.Select(x => x.ErrorMessage).Distinct().ToArray());
        }

        if (!SubscriptionPlanCatalog.TryGet(request.PlanCode, out var plan) || plan is null)
        {
            return OperationalResult<SubscriptionSummaryResult>.Fail(
                "Plan is not supported",
                "PLAN_NOT_FOUND",
                $"Unknown plan code '{request.PlanCode}'.");
        }

        try
        {
            var user = appUserQueryRepository.Get(request.UserId);
            if (user is null)
            {
                return OperationalResult<SubscriptionSummaryResult>.Fail(
                    "User was not found",
                    "USER_NOT_FOUND",
                    $"Unknown user '{request.UserId}'.");
            }

            var purchasedAtUtc = DateTime.UtcNow;
            var existingSubscription = await subscriptionQueryRepository.GetByUserIdAsync(request.UserId, cancellationToken);
            var basePriceUsd = request.BillingCycle == Domain.Enums.SubscriptionBillingCycle.Yearly
                ? plan.YearlyBasePriceUsd
                : plan.MonthlyPriceUsd;
            var discountPercentage = request.BillingCycle == Domain.Enums.SubscriptionBillingCycle.Yearly
                ? plan.YearlyDiscountPercentage
                : 0m;
            var finalPriceUsd = request.BillingCycle == Domain.Enums.SubscriptionBillingCycle.Yearly
                ? plan.YearlyPriceUsd
                : plan.MonthlyPriceUsd;

            if (existingSubscription is null)
            {
                existingSubscription = new Subscription(
                    Guid.NewGuid(),
                    request.UserId,
                    plan.PlanCode,
                    plan.PlanName,
                    request.BillingCycle,
                    basePriceUsd,
                    discountPercentage,
                    finalPriceUsd,
                    purchasedAtUtc);

                subscriptionCommandRepository.Create(existingSubscription);
            }
            else
            {
                existingSubscription.Purchase(
                    plan.PlanCode,
                    plan.PlanName,
                    request.BillingCycle,
                    basePriceUsd,
                    discountPercentage,
                    finalPriceUsd,
                    purchasedAtUtc);
            }

            subscriptionCommandRepository.SaveChanges();

            return OperationalResult<SubscriptionSummaryResult>.Success(
                MapToResult(existingSubscription, purchasedAtUtc),
                "Subscription activated successfully.");
        }
        catch (KeyNotFoundException)
        {
            return OperationalResult<SubscriptionSummaryResult>.Fail(
                "User was not found",
                "USER_NOT_FOUND",
                $"Unknown user '{request.UserId}'.");
        }
        catch (Exception ex)
        {
            return OperationalResult<SubscriptionSummaryResult>.Fail(
                "Unexpected error",
                "UNEXPECTED",
                ex.Message);
        }
    }

    public static SubscriptionSummaryResult MapToResult(Subscription subscription, DateTime utcNow)
    {
        return new SubscriptionSummaryResult(
            subscription.Id,
            subscription.UserId,
            subscription.PlanCode,
            subscription.PlanName,
            subscription.BillingCycle.ToString().ToLowerInvariant(),
            subscription.ResolveStatus(utcNow).ToString().ToLowerInvariant(),
            subscription.BasePriceUsd,
            subscription.DiscountPercentage,
            subscription.FinalPriceUsd,
            subscription.StartsAtUtc,
            subscription.ExpiresAtUtc);
    }
}
