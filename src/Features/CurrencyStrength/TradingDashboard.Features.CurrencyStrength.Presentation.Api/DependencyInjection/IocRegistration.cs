using FluentValidation;
using MediatR;
using TradingDashboard.Application.Common.Results;
using TradingDashboard.Application.Features.CurrencyStrength.Queries.GetCurrencyStrengthSnapshot;
using TradingDashboard.Application.Features.CurrencyStrength.Queries.GetCurrencyStrengthTimeframes;
using TradingDashboard.Application.Repository.CurrencyStrength;
using TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

namespace TradingDashboard.Presentation.Api.CurrencyStrength.DependencyInjection;

public static class IocRegistration
{
    public static IServiceCollection AddIocRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ICurrencyStrengthSnapshotQueryRepository>(_ =>
            new CurrencyStrengthFileQueryRepository(BuildStorageRoots(configuration)));

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(IocRegistration).Assembly));
        services.AddTransient<IRequestHandler<GetCurrencyStrengthTimeframesQuery, OperationalResult<GetCurrencyStrengthTimeframesResponse>>, GetCurrencyStrengthTimeframesHandler>();
        services.AddTransient<IRequestHandler<GetCurrencyStrengthSnapshotQuery, OperationalResult<GetCurrencyStrengthSnapshotResponse>>, GetCurrencyStrengthSnapshotHandler>();
        services.AddScoped<IValidator<GetCurrencyStrengthSnapshotQuery>, GetCurrencyStrengthSnapshotValidator>();

        return services;
    }

    private static IReadOnlyList<string> BuildStorageRoots(IConfiguration configuration)
    {
        var roots = new List<string>();
        var configuredRoot = configuration["CurrencyStrengthStorage:Root"];

        if (!string.IsNullOrWhiteSpace(configuredRoot))
        {
            roots.Add(Path.IsPathRooted(configuredRoot)
                ? configuredRoot
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuredRoot)));
        }

        roots.Add(Path.Combine(AppContext.BaseDirectory, "storage", "currency-strength"));

        var collectServiceBinRoot = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "TradingDashboard.Features.CurrencyStrength.CollectService",
            "bin"));

        if (Directory.Exists(collectServiceBinRoot))
        {
            roots.AddRange(Directory.GetDirectories(collectServiceBinRoot, "currency-strength", SearchOption.AllDirectories));
        }

        return roots
            .Select(Path.GetFullPath)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
