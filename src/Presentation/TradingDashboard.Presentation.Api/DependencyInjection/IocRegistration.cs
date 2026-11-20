using FluentValidation;
using MediatR;
using TradingDashboard.Application.Features.Auth.Commands.SignIn;
using CotIocRegistration = TradingDashboard.Presentation.Api.Cot.DependencyInjection.IocRegistration;
using CurrencyStrengthIocRegistration = TradingDashboard.Presentation.Api.CurrencyStrength.DependencyInjection.IocRegistration;
using UserManagementIocRegistration = TradingDashboard.Presentation.Api.UserManagement.DependencyInjection.IocRegistration;

namespace TradingDashboard.Presentation.Api.DependencyInjection;

public static class IocRegistration
{
    public static IServiceCollection AddIocRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        var applicationAssembly = typeof(SignInCommand).Assembly;
        services.AddMediatR(options => options.RegisterServicesFromAssembly(applicationAssembly));
        services.AddValidatorsFromAssembly(applicationAssembly);

        CotIocRegistration.AddIocRegistration(services, configuration);
        CurrencyStrengthIocRegistration.AddIocRegistration(services, configuration);
        UserManagementIocRegistration.AddIocRegistration(services, configuration);

        return services;
    }
}
