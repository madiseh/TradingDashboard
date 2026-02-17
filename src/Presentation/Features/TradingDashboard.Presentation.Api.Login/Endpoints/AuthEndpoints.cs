using System.Security.Claims;
using MediatR;
using TradingDashboard.Application.Features.Auth.Commands.SignIn;
using TradingDashboard.Application.Features.Auth.Commands.SignUp;
using TradingDashboard.Presentation.Api.Login.Contracts;
using TradingDashboard.Presentation.Api.Login.Services;

namespace TradingDashboard.Presentation.Api.Login.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RoutePaths.Api.Login.LoginBase)
            .WithTags("Auth");

        group.MapPost("signup", SignUpAsync)
            .WithName("AuthSignUp")
            .WithSummary("Create a new account and returns an access token");

        group.MapPost("signin", SignInAsync)
            .WithName("AuthSignIn")
            .WithSummary("Sign in with username/email and password");

        group.MapGet("me", GetCurrentUser)
            .WithName("AuthMe")
            .RequireAuthorization()
            .WithSummary("Returns the current authenticated user");

        return app;
    }

    private static async Task<IResult> SignUpAsync(
        SignUpRequest request,
        IMediator mediator,
        IJwtTokenService jwtTokenService,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new SignUpCommand(request.Username, request.Email, request.Password),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return TypedResults.BadRequest(new { result.Message, result.ErrorCode, result.Errors });
        }

        var token = jwtTokenService.GenerateToken(result.Data);
        return TypedResults.Ok(token);
    }

    private static async Task<IResult> SignInAsync(
        SignInRequest request,
        IMediator mediator,
        IJwtTokenService jwtTokenService,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new SignInCommand(request.Identity, request.Password),
            cancellationToken);

        if (!result.IsSuccess || result.Data is null)
        {
            return TypedResults.BadRequest(new { result.Message, result.ErrorCode, result.Errors });
        }

        var token = jwtTokenService.GenerateToken(result.Data);
        return TypedResults.Ok(token);
    }

    private static IResult GetCurrentUser(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(ClaimTypes.Name);
        var username = user.FindFirstValue(ClaimTypes.Name) ?? user.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName);
        var email = user.FindFirstValue(ClaimTypes.Email) ?? user.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email);

        return TypedResults.Ok(new
        {
            UserId = userId,
            Username = username,
            Email = email
        });
    }
}
