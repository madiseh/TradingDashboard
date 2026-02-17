using MediatR;
using TradingDashboard.Application.Common.Results;

namespace TradingDashboard.Application.Features.Auth.Commands.SignIn;

public sealed record SignInCommand(string Identity, string Password)
    : IRequest<OperationalResult<AuthUserResponse>>;
