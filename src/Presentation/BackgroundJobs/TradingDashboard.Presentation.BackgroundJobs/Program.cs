using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Quartz;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Context;
using TradingDashboard.Presentation.BackgroundJobs.Scheduler;

namespace TradingDashboard.Presentation.BackgroundJobs;

public static class Program
{
    public static async Task Main(string[] args)
    {
        while (true)
        {
            try
            {
                await RunAsync(args);
            }
            catch (Exception exception)
            {
                Console.WriteLine($"BackgroundJobs host crashed: {exception}");
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }

    public static async Task AddJob_HelloWorld(IScheduler scheduler, BackgroundJobsDbContext dbContext)
    {
        var job = JobBuilder.Create<HelloWorldJob>()
            .WithIdentity("hello-world-job")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("hello-world-trigger")
            .StartNow()
            .Build();

        await BackgroundJobRegistrar.AddJobAsync(scheduler, dbContext, job, trigger);
    }

    private static void AddPersistence(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BackgroundJobsDatabase");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'BackgroundJobsDatabase' is required.");
        }

        services.AddDbContext<BackgroundJobsDbContext>(options =>
            options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure()));
    }

    private static async Task RunAsync(string[] args)
    {
        var builder = new HostApplicationBuilder(new HostApplicationBuilderSettings
        {
            Args = args,
            ApplicationName = typeof(Program).Assembly.FullName,
            ContentRootPath = AppContext.BaseDirectory
        });

        AddPersistence(builder.Services, builder.Configuration);

        using var host = builder.Build();

        await using var scope = host.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BackgroundJobsDbContext>();
        await dbContext.Database.MigrateAsync();

        var scheduler = await QuartzSchedulerBuilder.CreateAsync(scope.ServiceProvider);
        await AddJob_HelloWorld(scheduler, dbContext);
        await scheduler.Start();

        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopping.Register(() =>
        {
            scheduler.Shutdown(waitForJobsToComplete: true).GetAwaiter().GetResult();
        });

        await host.RunAsync();
    }
}
