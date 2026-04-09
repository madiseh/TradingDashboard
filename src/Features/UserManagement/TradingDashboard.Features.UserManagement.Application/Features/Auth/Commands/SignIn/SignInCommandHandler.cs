using FluentValidation;
using MediatR;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Common.Security;
using TradingDashboard.Application.Repository.Auth;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Features.Auth.Commands.SignIn;

public sealed class SignInCommandHandler(
    IAppUserQueryRepository userQueryRepository,
    IAppUserCommandRepository userCommandRepository,
    IPasswordHashingService passwordHashingService,
    IValidator<SignInCommand> validator)
    : IRequestHandler<SignInCommand, OperationalResult<AuthUserResponse>>
{
    public async Task<OperationalResult<AuthUserResponse>> Handle(SignInCommand request, CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return OperationalResult<AuthUserResponse>.Fail(
                "Validation failed",
                "VALIDATION",
                validation.Errors.Select(x => x.ErrorMessage).Distinct().ToArray());
        }

        try
        {
            var user = await userQueryRepository.GetByIdentityAsync(request.Identity, cancellationToken);
            if (user is null)
            {
                return OperationalResult<AuthUserResponse>.Fail(
                    "Invalid credentials",
                    "INVALID_CREDENTIALS",
                    "Username/email or password is invalid.");
            }

            if (user.Status != AppUserStatus.Active)
            {
                return OperationalResult<AuthUserResponse>.Fail(
                    "Account is disabled",
                    "ACCOUNT_DISABLED",
                    "This account is disabled and cannot sign in.");
            }

            var isPasswordValid = passwordHashingService.Verify(request.Password, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordValid)
            {
                return OperationalResult<AuthUserResponse>.Fail(
                    "Invalid credentials",
                    "INVALID_CREDENTIALS",
                    "Username/email or password is invalid.");
            }

            user.MarkLogin(DateTime.UtcNow);
            userCommandRepository.SaveChanges();

            return OperationalResult<AuthUserResponse>.Success(
                new AuthUserResponse(user.Id, user.Username, user.Email.Value, user.Role),
                "Signed in successfully.");
        }
        catch (Exception ex)
        {
            return OperationalResult<AuthUserResponse>.Fail(
                "Unexpected error",
                "UNEXPECTED",
                ex.Message);
        }
    }
}
