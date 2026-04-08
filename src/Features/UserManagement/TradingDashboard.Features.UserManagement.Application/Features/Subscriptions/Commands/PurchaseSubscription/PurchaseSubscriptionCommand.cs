using MediatR;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Features.Subscriptions.Commands.PurchaseSubscription;

public sealed record PurchaseSubscriptionCommand(
    Guid UserId,
    string PlanCode,
    SubscriptionBillingCycle BillingCycle) : IRequest<OperationalResult<SubscriptionSummaryResult>>;
