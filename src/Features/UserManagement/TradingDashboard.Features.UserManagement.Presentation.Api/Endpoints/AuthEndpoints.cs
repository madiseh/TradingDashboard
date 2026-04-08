using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.WebUtilities;
using MediatR;
using TradingDashboard.Application.Common.Security;
using TradingDashboard.Application.Features.Auth.Commands.SignIn;
using TradingDashboard.Application.Features.Auth.Commands.SignUp;
using TradingDashboard.Application.Features.Auth;
using TradingDashboard.Application.Repository.Auth;
using TradingDashboard.Domain.Entities;
using TradingDashboard.Domain.Enums;
using TradingDashboard.Presentation.Api.Login.Contracts;
using TradingDashboard.Presentation.Api.Login.Services;
using TradingDashboard.Presentation.Common.Contracts;

namespace TradingDashboard.Presentation.Api.Login.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(RoutePaths.Api.Login.LoginBase)
            .WithTags("Auth");

        group.MapPost("signup", SignUpAsync)
            .WithName("AuthSignUp")
            .WithSummary("Create a new account and returns an access token")
            .Produces<AuthTokenResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapPost("signin", SignInAsync)
            .WithName("AuthSignIn")
            .WithSummary("Sign in with username/email and password")
            .Produces<AuthTokenResult>(StatusCodes.Status200OK)
            .Produces<ApiErrorResponse>(StatusCodes.Status400BadRequest);

        group.MapGet("me", GetCurrentUser)
            .WithName("AuthMe")
            .RequireAuthorization()
            .WithSummary("Returns the current authenticated user")
            .Produces<MeResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized);

        group.MapGet("google/start", BeginGoogleSignInAsync)
            .WithName("AuthGoogleSignInStart")
            .WithSummary("Starts Google sign-in flow");

        group.MapGet("google/callback", CompleteGoogleSignInAsync)
            .WithName("AuthGoogleSignInCallback")
            .WithSummary("Completes Google sign-in and redirects to UI login page");

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
            return TypedResults.BadRequest(new ApiErrorResponse(result.Message, result.ErrorCode, result.Errors));
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
            return TypedResults.BadRequest(new ApiErrorResponse(result.Message, result.ErrorCode, result.Errors));
        }

        var token = jwtTokenService.GenerateToken(result.Data);
        return TypedResults.Ok(token);
    }

    private static IResult GetCurrentUser(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(ClaimTypes.Name);
        var username = user.FindFirstValue(ClaimTypes.Name) ?? user.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.UniqueName);
        var email = user.FindFirstValue(ClaimTypes.Email) ?? user.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email);

        return TypedResults.Ok(new MeResponse(userId, username, email));
    }

    private static async Task<IResult> BeginGoogleSignInAsync(
        HttpContext httpContext,
        IAuthenticationSchemeProvider authenticationSchemeProvider,
        string? returnUrl,
        CancellationToken cancellationToken)
    {
        var normalizedReturnUrl = NormalizeReturnUrl(returnUrl);
        if (string.IsNullOrWhiteSpace(normalizedReturnUrl))
        {
            return TypedResults.BadRequest(new ApiErrorResponse("The returnUrl query parameter is required.", "MISSING_RETURN_URL", null));
        }

        var googleScheme = await authenticationSchemeProvider.GetSchemeAsync("Google");
        if (googleScheme is null)
        {
            return TypedResults.BadRequest(new ApiErrorResponse(
                "Google authentication is not configured. Set Authentication:Google:ClientId and ClientSecret.",
                "GOOGLE_AUTH_NOT_CONFIGURED",
                null));
        }

        var callbackUrl = QueryHelpers.AddQueryString(RoutePaths.Api.Login.GoogleCallback, "returnUrl", normalizedReturnUrl);
        var properties = new AuthenticationProperties
        {
            RedirectUri = callbackUrl
        };

        return Results.Challenge(properties, ["Google"]);
    }

    private static async Task<IResult> CompleteGoogleSignInAsync(
        HttpContext httpContext,
        IAppUserQueryRepository userQueryRepository,
        IAppUserCommandRepository userCommandRepository,
        IPasswordHashingService passwordHashingService,
        IJwtTokenService jwtTokenService,
        string? returnUrl,
        CancellationToken cancellationToken)
    {
        var normalizedReturnUrl = NormalizeReturnUrl(returnUrl);
        if (string.IsNullOrWhiteSpace(normalizedReturnUrl))
        {
            return TypedResults.BadRequest(new ApiErrorResponse("The returnUrl query parameter is required.", "MISSING_RETURN_URL", null));
        }

        var authResult = await httpContext.AuthenticateAsync("External");
        if (!authResult.Succeeded || authResult.Principal is null)
        {
            var failedUrl = QueryHelpers.AddQueryString(normalizedReturnUrl, "authError", "Google authentication failed.");
            return Results.Redirect(failedUrl);
        }

        var principal = authResult.Principal;
        var email = principal.FindFirstValue(ClaimTypes.Email)
            ?? principal.FindFirstValue(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email);

        if (string.IsNullOrWhiteSpace(email))
        {
            await httpContext.SignOutAsync("External");
            var failedUrl = QueryHelpers.AddQueryString(normalizedReturnUrl, "authError", "Google account email was not provided.");
            return Results.Redirect(failedUrl);
        }

        var user = await userQueryRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            var displayName = principal.FindFirstValue(ClaimTypes.Name) ?? principal.FindFirstValue("name");
            var baseUsername = BuildBaseUsername(displayName, email);
            var uniqueUsername = await GenerateUniqueUsernameAsync(baseUsername, userQueryRepository, cancellationToken);
            var (passwordHash, passwordSalt) = passwordHashingService.CreateHash($"google-{Guid.NewGuid():N}");

            user = new AppUser(
                Guid.NewGuid(),
                uniqueUsername,
                email,
                passwordHash,
                passwordSalt,
                AppUserRole.Guest,
                AppUserStatus.Active,
                SignupAppPlatform.Google,
                true,
                DateTime.UtcNow);

            user.MarkLogin(DateTime.UtcNow);
            userCommandRepository.Create(user);
            userCommandRepository.SaveChanges();
        }
        else
        {
            if (user.Status != AppUserStatus.Active)
            {
                await httpContext.SignOutAsync("External");
                var failedUrl = QueryHelpers.AddQueryString(normalizedReturnUrl, "authError", "This account is disabled.");
                return Results.Redirect(failedUrl);
            }

            user.MarkLogin(DateTime.UtcNow);
            userCommandRepository.SaveChanges();
        }

        await httpContext.SignOutAsync("External");

        var tokenResult = jwtTokenService.GenerateToken(new AuthUserResponse(user.Id, user.Username, user.Email.Value, user.Role.ToString()));
        var redirectUrl = QueryHelpers.AddQueryString(normalizedReturnUrl, new Dictionary<string, string?>
        {
            ["accessToken"] = tokenResult.AccessToken,
            ["expiresAtUtc"] = tokenResult.ExpiresAtUtc.ToString("O"),
            ["userId"] = tokenResult.UserId.ToString(),
            ["username"] = tokenResult.Username,
            ["email"] = tokenResult.Email
        });

        return Results.Redirect(redirectUrl);
    }

    private static string? NormalizeReturnUrl(string? returnUrl)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return null;
        }

        var normalized = returnUrl.Trim();
        if (!Uri.TryCreate(normalized, UriKind.Absolute, out var absoluteUri))
        {
            return null;
        }

        return absoluteUri.Scheme is "https" or "http"
            ? absoluteUri.ToString()
            : null;
    }

    private static string BuildBaseUsername(string? displayName, string email)
    {
        var source = string.IsNullOrWhiteSpace(displayName)
            ? email.Split('@')[0]
            : displayName;

        var filtered = new string(source
            .Trim()
            .Where(ch => char.IsLetterOrDigit(ch) || ch == '_' || ch == '.')
            .ToArray());

        return string.IsNullOrWhiteSpace(filtered) ? $"user_{Guid.NewGuid():N}"[..14] : filtered;
    }

    private static async Task<string> GenerateUniqueUsernameAsync(
        string baseUsername,
        IAppUserQueryRepository userQueryRepository,
        CancellationToken cancellationToken)
    {
        var candidate = baseUsername;
        var index = 0;

        while (true)
        {
            var existing = await userQueryRepository.GetByUsernameAsync(candidate, cancellationToken);
            if (existing is null)
            {
                return candidate;
            }

            index++;
            candidate = $"{baseUsername}{index}";
        }
    }
}
