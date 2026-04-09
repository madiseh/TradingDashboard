using FluentValidation;
using MediatR;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Common.Security;
using TradingDashboard.Application.Repository.Auth;

namespace TradingDashboard.Application.Features.AdminUsers.Commands.ChangeUserPassword;

public sealed class ChangeUserPasswordCommandHandler(
    IAppUserQueryRepository userQueryRepository,
    IAppUserCommandRepository userCommandRepository,
    IPasswordHashingService passwordHashingService,
    IValidator<ChangeUserPasswordCommand> validator)
    : IRequestHandler<ChangeUserPasswordCommand, OperationalResult<ChangeUserPasswordResponse>>
{
    public async Task<OperationalResult<ChangeUserPasswordResponse>> Handle(
        ChangeUserPasswordCommand request,
        CancellationToken cancellationToken)
    {
        var validation = await validator.ValidateAsync(request, cancellationToken);
        if (!validation.IsValid)
        {
            return OperationalResult<ChangeUserPasswordResponse>.Fail(
                "Validation failed",
                "VALIDATION",
                validation.Errors.Select(x => x.ErrorMessage).Distinct().ToArray());
        }

        try
        {
            var user = await userQueryRepository.GetByIdAsync(request.UserId, cancellationToken);
            if (user is null)
            {
                return OperationalResult<ChangeUserPasswordResponse>.Fail(
                    "User not found",
                    "USER_NOT_FOUND",
                    "The selected user no longer exists.");
            }

            var (passwordHash, passwordSalt) = passwordHashingService.CreateHash(request.NewPassword);
            user.ChangePassword(passwordHash, passwordSalt, DateTime.UtcNow);
            userCommandRepository.SaveChanges();

            return OperationalResult<ChangeUserPasswordResponse>.Success(
                new ChangeUserPasswordResponse(user.Id, "Password updated successfully."),
                "Password updated successfully.");
        }
        catch (Exception ex)
        {
            return OperationalResult<ChangeUserPasswordResponse>.Fail(
                "Unexpected error",
                "UNEXPECTED",
                ex.Message);
        }
    }
}
