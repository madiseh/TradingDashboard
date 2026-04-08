using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Quartz;
using TradingDashboard.Infrastructure.Persistence.Implementation.Context;
using TradingDashboard.Infrastructure.Persistence.Implementation.Seed;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Context;
using TradingDashboard.Presentation.BackgroundJobs.Scheduler;

namespace TradingDashboard.Presentation.BackgroundJobs.Jobs.Cot;

public static class CotJobRegistration
{
    public const string HttpClientName = "cot-collector";

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("CotDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'CotDatabase' is required.");
        }

        services.Configure<CotCollectorOptions>(
            configuration.GetSection(CotCollectorOptions.SectionName));

        services.AddDbContext<CotDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));

        services.AddHttpClient(HttpClientName, (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<CotCollectorOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds);
        });

        services.AddTransient<CotCollectionRunner>();
        services.AddTransient<CotCollectJob>();
    }

    public static async Task EnsureInfrastructureAsync(
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CotDbContext>();
        await dbContext.Database.MigrateAsync(cancellationToken);
        await EnsureCollectorStateTableAsync(dbContext, cancellationToken);
        CotInstrumentSeedBootstrapper.SeedMissingInstruments(dbContext);
    }

    public static async Task RegisterAsync(
        IScheduler scheduler,
        BackgroundJobsDbContext dbContext,
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        var options = services.GetRequiredService<IOptions<CotCollectorOptions>>().Value;

        if (options.IntervalMinutes <= 0)
        {
            throw new InvalidOperationException("CotCollector:IntervalMinutes must be greater than zero.");
        }

        var job = JobBuilder.Create<CotCollectJob>()
            .WithIdentity("cot-collect-job")
            .Build();

        var startAt = options.RunOnStartup
            ? DateTimeOffset.UtcNow
            : DateTimeOffset.UtcNow.AddMinutes(options.IntervalMinutes);

        var trigger = TriggerBuilder.Create()
            .WithIdentity("cot-collect-trigger")
            .StartAt(startAt)
            .WithSimpleSchedule(schedule => schedule
                .WithInterval(TimeSpan.FromMinutes(options.IntervalMinutes))
                .RepeatForever())
            .Build();

        await BackgroundJobRegistrar.AddJobAsync(scheduler, dbContext, job, trigger, cancellationToken);
    }

    private static Task EnsureCollectorStateTableAsync(
        CotDbContext dbContext,
        CancellationToken cancellationToken)
    {
        const string sql = """
                           IF SCHEMA_ID(N'cot') IS NULL
                           BEGIN
                               EXEC(N'CREATE SCHEMA [cot]');
                           END;

                           IF OBJECT_ID(N'[cot].[CotCollectionJobStates]', N'U') IS NULL
                           BEGIN
                               CREATE TABLE [cot].[CotCollectionJobStates] (
                                   [Id] nvarchar(128) NOT NULL,
                                   [LastStartedAtUtc] datetime2 NULL,
                                   [LastCompletedAtUtc] datetime2 NULL,
                                   [LastSucceededAtUtc] datetime2 NULL,
                                   [LastSavedFileCount] int NOT NULL CONSTRAINT [DF_CotCollectionJobStates_LastSavedFileCount] DEFAULT (0),
                                   [LastFetchedProfileCount] int NOT NULL CONSTRAINT [DF_CotCollectionJobStates_LastFetchedProfileCount] DEFAULT (0),
                                   [LastError] nvarchar(2048) NULL,
                                   [CreatedAtUtc] datetime2 NOT NULL,
                                   [UpdatedAtUtc] datetime2 NOT NULL,
                                   CONSTRAINT [PK_CotCollectionJobStates] PRIMARY KEY ([Id])
                               );
                           END;
                           """;

        return dbContext.Database.ExecuteSqlRawAsync(sql, cancellationToken);
    }
}
