using FluentValidation;
using MediatR;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Common.Security;
using TradingDashboard.Application.Repository.Auth;
using TradingDashboard.Domain.Entities;
using TradingDashboard.Domain.Enums;

namespace TradingDashboard.Application.Features.Auth.Commands.SignUp;

public sealed class SignUpCommandHandler(
    IAppUserQueryRepository userQueryRepository,
    IAppUserCommandRepository userCommandRepository,
    IPasswordHashingService passwordHashingService,
    IValidator<SignUpCommand> validator)
    : IRequestHandler<SignUpCommand, OperationalResult<AuthUserResponse>>
{
    public async Task<OperationalResult<AuthUserResponse>> Handle(SignUpCommand request, CancellationToken cancellationToken)
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
            var existingByUsername = await userQueryRepository.GetByUsernameAsync(request.Username, cancellationToken);
            if (existingByUsername is not null)
            {
                return OperationalResult<AuthUserResponse>.Fail(
                    "Username already exists",
                    "DUPLICATE_USERNAME",
                    "Choose another username.");
            }

            var existingByEmail = await userQueryRepository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingByEmail is not null)
            {
                return OperationalResult<AuthUserResponse>.Fail(
                    "Email already exists",
                    "DUPLICATE_EMAIL",
                    "This email is already registered.");
            }

            var (passwordHash, passwordSalt) = passwordHashingService.CreateHash(request.Password);

            var user = new AppUser(
                Guid.NewGuid(),
                request.Username,
                request.Email,
                passwordHash,
                passwordSalt,
                AppUserRole.Guest,
                AppUserStatus.Active,
                SignupAppPlatform.Local,
                false,
                DateTime.UtcNow);

            userCommandRepository.Create(user);
            userCommandRepository.SaveChanges();

            return OperationalResult<AuthUserResponse>.Success(
                new AuthUserResponse(user.Id, user.Username, user.Email.Value, user.Role),
                "Account created successfully.");
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
