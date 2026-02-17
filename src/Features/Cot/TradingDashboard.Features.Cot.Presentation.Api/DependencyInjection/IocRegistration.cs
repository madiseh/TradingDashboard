using FluentValidation;
using MediatR;
using TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentBySlug;
using TradingDashboard.Application.Features.CotInstruments.Queries.GetCotInstrumentsByGroup;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Repository.Cot;
using TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

namespace TradingDashboard.Presentation.Api.Cot.DependencyInjection;

public static class IocRegistration
{
    public static IServiceCollection AddIocRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CotDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'CotDatabase' is required.");
        }

        services.AddDbContext<CotDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure();
            }));


        services.AddIocRegistrationRepository();
        services.AddScoped<ICotInstrumentQueryRepository, CotInstrumentQueryRepository>();
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IocRegistration).Assembly));

        services.AddTransient<IRequestHandler<GetCotInstrumentBySlugQuery, OperationalResult<GetCotInstrumentBySlugResponse>>, GetCotInstrumentBySlugHandler>();
        services.AddTransient<IRequestHandler<GetCotInstrumentsByGroupQuery, OperationalResult<IReadOnlyList<GetCotInstrumentsByGroupResponse>>>, GetCotInstrumentsByGroupHandler>();

        services.AddScoped<IValidator<GetCotInstrumentBySlugQuery>, GetCotInstrumentBySlugValidator>();
        services.AddScoped<IValidator<GetCotInstrumentsByGroupQuery>, GetCotInstrumentsByGroupValidator>();


        return services;
    }
}
