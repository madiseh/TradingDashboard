using TradingDashboard.Application.Repository.Cot;
using TradingDashboard.Infrastructure.Persistence.Implementation.Repositories;

namespace TradingDashboard.Presentation.Api.Cot.DependencyInjection;

public static class IocRegistration
{
    public static IServiceCollection AddIocRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CotDatabase");
        var skipDbInit = string.Equals(
            Environment.GetEnvironmentVariable("TD_SKIP_DB_INIT"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            if (skipDbInit)
            {
                connectionString = "Server=(localdb)\\mssqllocaldb;Database=TradingDashboard_Cot_Swagger;Trusted_Connection=True;TrustServerCertificate=True;";
            }
            else
            {
                throw new InvalidOperationException("Connection string 'CotDatabase' is required.");
            }
        }

        services.AddDbContext<CotDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.EnableRetryOnFailure();
            }));


        services.AddIocRegistrationRepository();
        services.AddScoped<ICotInstrumentQueryRepository, CotInstrumentQueryRepository>();
        services.AddScoped<ICotLatestReportQueryRepository>(_ => new CotLatestReportFileQueryRepository(BuildCotRawStorageRoots(configuration)));
        services.AddScoped<ICotHistoricalReportQueryRepository>(_ => new CotHistoricalReportFileQueryRepository(BuildCotRawStorageRoots(configuration)));
        return services;
    }

    private static IReadOnlyList<string> BuildCotRawStorageRoots(IConfiguration configuration)
    {
        var roots = new List<string>();
        var configuredRoot = configuration["CotRawStorage:Root"];

        if (!string.IsNullOrWhiteSpace(configuredRoot))
        {
            roots.Add(Path.IsPathRooted(configuredRoot)
                ? configuredRoot
                : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, configuredRoot)));
        }

        roots.Add(Path.Combine(AppContext.BaseDirectory, "storage", "cot-raw"));

        var backgroundJobsBinRoot = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "..",
            "..",
            "Presentation",
            "BackgroundJobs",
            "TradingDashboard.Presentation.BackgroundJobs",
            "bin"));

        if (Directory.Exists(backgroundJobsBinRoot))
        {
            roots.AddRange(Directory.GetDirectories(backgroundJobsBinRoot, "cot-raw", SearchOption.AllDirectories));
        }

        var collectServiceBinRoot = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            "TradingDashboard.Features.Cot.CollectService",
            "bin"));

        if (Directory.Exists(collectServiceBinRoot))
        {
            roots.AddRange(Directory.GetDirectories(collectServiceBinRoot, "cot-raw", SearchOption.AllDirectories));
        }

        return roots
            .Select(Path.GetFullPath)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
