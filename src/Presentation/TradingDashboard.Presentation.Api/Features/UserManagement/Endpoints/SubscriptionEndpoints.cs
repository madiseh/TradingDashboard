using System.Security.Claims;
using MediatR;
using TradingDashboard.Application.Common.Subscriptions;
using TradingDashboard.Application.Features.Subscriptions;
using TradingDashboard.Application.Features.Subscriptions.Commands.PurchaseSubscription;
using TradingDashboard.Application.Repository.Subscriptions;
using TradingDashboard.Domain.Enums;
using TradingDashboard.Presentation.Api.UserManagement.Contracts;
using TradingDashboard.Presentation.Common.Contracts;

namespace TradingDashboard.Presentation.Api.UserManagement.Endpoints;

public static class SubscriptionEndpoints
{
    public static IEndpointRouteBuilder MapSubscriptionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RoutePaths.Api.Login.SubscriptionsBase)
            .WithTags("Subscriptions");

        group.MapGet("plans", GetPlans)
            .WithName("GetSubscriptionPlans")
            .WithSummary("Returns supported subscription plans")
            .Produces<IReadOnlyList<SubscriptionPlanResponse>>(StatusCodes.Status200OK);

        group.MapGet("me", GetCurrentSubscriptionAsync)
            .WithName("GetCurrentSubscription")
            .RequireAuthorization()
            .WithSummary("Returns the current authenticated user subscription")
            .Produces<SubscriptionResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapPost("purchase", PurchaseSubscriptionAsync)
            .WithName("PurchaseSubscription")
            .RequireAuthorization()
            .WithSummary("Activates or extends a subscription for the authenticated user")
            .Produces<SubscriptionResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized);

        return app;
    }

    private static IResult GetPlans()
    {
        var response = SubscriptionPlanCatalog.GetPlans()
            .Select(plan => new SubscriptionPlanResponse(
                plan.PlanCode,
                plan.PlanName,
                plan.MonthlyPriceUsd,
                plan.YearlyPriceUsd,
                plan.YearlyDiscountPercentage))
            .ToArray();

        return TypedResults.Ok<IReadOnlyList<SubscriptionPlanResponse>>(response);
    }

    private static async Task<IResult> GetCurrentSubscriptionAsync(
        ClaimsPrincipal user,
        ISubscriptionQueryRepository subscriptionQueryRepository,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(user, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        var subscription = await subscriptionQueryRepository.GetByUserIdAsync(userId, cancellationToken);
        if (subscription is null)
        {
            return TypedResults.NoContent();
        }

        var result = PurchaseSubscriptionCommandHandler.MapToResult(subscription, DateTime.UtcNow);
        return TypedResults.Ok(MapToResponse(result));
    }

    private static async Task<IResult> PurchaseSubscriptionAsync(
        PurchaseSubscriptionRequest request,
        ClaimsPrincipal user,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        if (!TryGetUserId(user, out var userId))
        {
            return TypedResults.Unauthorized();
        }

        if (!Enum.TryParse<SubscriptionBillingCycle>(request.BillingCycle, true, out var billingCycle))
        {
            return TypedResults.BadRequest(new ApiErrorResponse(
                "Billing cycle is invalid.",
                "INVALID_BILLING_CYCLE",
                ["Use monthly or yearly."]));
        }

        var result = await mediator.Send(
            new PurchaseSubscriptionCommand(userId, request.PlanCode, billingCycle),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return TypedResults.BadRequest(new ApiErrorResponse(result.Message, result.ErrorCode, result.Errors));
        }

        return TypedResults.Ok(MapToResponse(result.Data));
    }

    private static SubscriptionResponse MapToResponse(SubscriptionSummaryResult result)
    {
        return new SubscriptionResponse(
            result.SubscriptionId,
            result.UserId,
            result.PlanCode,
            result.PlanName,
            result.BillingCycle,
            result.Status,
            result.BasePriceUsd,
            result.DiscountPercentage,
            result.FinalPriceUsd,
            result.StartsAtUtc,
            result.ExpiresAtUtc);
    }

    private static bool TryGetUserId(ClaimsPrincipal user, out Guid userId)
    {
        var rawUserId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(ClaimTypes.Name);
        return Guid.TryParse(rawUserId, out userId);
    }
}
