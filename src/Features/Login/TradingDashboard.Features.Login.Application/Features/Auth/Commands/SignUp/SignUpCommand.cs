using MediatR;
using TradingDashboard.Application.Common.Results;

namespace TradingDashboard.Application.Features.Auth.Commands.SignUp;

public sealed record SignUpCommand(string Username, string Email, string Password)
    : IRequest<OperationalResult<AuthUserResponse>>;
