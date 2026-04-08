using Quartz;
using Microsoft.Extensions.Options;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Context;
using TradingDashboard.Presentation.BackgroundJobs.Scheduler;

namespace TradingDashboard.Presentation.BackgroundJobs.Jobs.CurrencyStrength;

public static class CurrencyStrengthJobRegistration
{
    public const string HttpClientName = "currency-strength-collector";

    public static void AddServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CurrencyStrengthCollectorOptions>(
            configuration.GetSection(CurrencyStrengthCollectorOptions.SectionName));

        services.AddHttpClient(HttpClientName, (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<CurrencyStrengthCollectorOptions>>().Value;
            client.Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds);
        });

        services.AddTransient<CurrencyStrengthCollectionRunner>();
        services.AddTransient<CurrencyStrengthCollectJob>();
    }

    public static async Task RegisterAsync(
        IScheduler scheduler,
        BackgroundJobsDbContext dbContext,
        IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        var options = services.GetRequiredService<IOptions<CurrencyStrengthCollectorOptions>>().Value;

        if (options.IntervalMinutes <= 0)
        {
            throw new InvalidOperationException("CurrencyStrengthCollector:IntervalMinutes must be greater than zero.");
        }

        var job = JobBuilder.Create<CurrencyStrengthCollectJob>()
            .WithIdentity("currency-strength-collect-job")
            .Build();

        var startAt = options.RunOnStartup
            ? DateTimeOffset.UtcNow
            : DateTimeOffset.UtcNow.AddMinutes(options.IntervalMinutes);

        var trigger = TriggerBuilder.Create()
            .WithIdentity("currency-strength-collect-trigger")
            .StartAt(startAt)
            .WithSimpleSchedule(schedule => schedule
                .WithInterval(TimeSpan.FromMinutes(options.IntervalMinutes))
                .RepeatForever())
            .Build();

        await BackgroundJobRegistrar.AddJobAsync(scheduler, dbContext, job, trigger, cancellationToken);
    }
}
