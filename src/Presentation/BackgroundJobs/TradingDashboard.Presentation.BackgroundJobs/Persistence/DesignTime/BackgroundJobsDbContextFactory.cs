using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Context;

namespace TradingDashboard.Presentation.BackgroundJobs.Persistence.DesignTime;

public sealed class BackgroundJobsDbContextFactory : IDesignTimeDbContextFactory<BackgroundJobsDbContext>
{
    public BackgroundJobsDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(ResolveConfigurationBasePath())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("BackgroundJobsDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'BackgroundJobsDatabase' is required.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<BackgroundJobsDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure());

        return new BackgroundJobsDbContext(optionsBuilder.Options);
    }

    private static string ResolveConfigurationBasePath()
    {
        var candidates = new[]
        {
            Directory.GetCurrentDirectory(),
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "src",
                "Presentation",
                "BackgroundJobs",
                "TradingDashboard.Presentation.BackgroundJobs"),
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."))
        };

        return candidates.First(path => File.Exists(Path.Combine(path, "appsettings.json")));
    }
}
