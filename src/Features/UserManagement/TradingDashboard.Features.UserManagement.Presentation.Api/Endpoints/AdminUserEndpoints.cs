using MediatR;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Features.AdminUsers.Commands.ChangeUserPassword;
using TradingDashboard.Application.Features.AdminUsers.Queries.GetAdminUsers;
using TradingDashboard.Domain.Enums;
using TradingDashboard.Presentation.Api.UserManagement.Contracts;
using TradingDashboard.Presentation.Common.Contracts;

namespace TradingDashboard.Presentation.Api.UserManagement.Endpoints;

public static class AdminUserEndpoints
{
    public static IEndpointRouteBuilder MapAdminUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RoutePaths.Api.Login.AdminUsers)
            .RequireAuthorization(policy => policy.RequireRole(AppUserRole.SuperAdmin.ToString()))
            .WithTags("Admin Users");

        group.MapGet(string.Empty, GetUsersAsync)
            .WithName("AdminUsersList")
            .WithSummary("Returns admin user list with optional search filter")
            .Produces<GetAdminUsersResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        group.MapPost("{userId:guid}/change-password", ChangePasswordAsync)
            .WithName("AdminUsersChangePassword")
            .WithSummary("Changes password for a user")
            .Produces<ChangeUserPasswordResponse>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest)
            .Produces<ApiErrorResponse>(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status403Forbidden);

        return app;
    }

    private static async Task<IResult> GetUsersAsync(
        string? search,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var response = await mediator.Send(new GetAdminUsersQuery(search), cancellationToken);
        return TypedResults.Ok(response);
    }

    private static async Task<IResult> ChangePasswordAsync(
        Guid userId,
        ChangeUserPasswordRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new ChangeUserPasswordCommand(userId, request.NewPassword),
            cancellationToken);

        if (result.IsSuccess && result.Data is not null)
        {
            return TypedResults.Ok(result.Data);
        }

        return result.ErrorCode == "USER_NOT_FOUND"
            ? TypedResults.NotFound(new ApiErrorResponse(result.Message, result.ErrorCode, result.Errors))
            : TypedResults.BadRequest(new ApiErrorResponse(result.Message, result.ErrorCode, result.Errors));
    }
}
