using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Quartz;
using TradingDashboard.Presentation.BackgroundJobs.Jobs.Cot;
using TradingDashboard.Presentation.BackgroundJobs.Jobs.CurrencyStrength;
using TradingDashboard.Presentation.BackgroundJobs.Jobs.HelloWorld;
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
        await HelloWorldJobRegistration.RegisterAsync(scheduler, dbContext);
    }

    public static async Task AddJob_CurrencyStrength(
        IScheduler scheduler,
        BackgroundJobsDbContext dbContext,
        IServiceProvider services)
    {
        await CurrencyStrengthJobRegistration.RegisterAsync(scheduler, dbContext, services);
    }

    public static async Task AddJob_Cot(
        IScheduler scheduler,
        BackgroundJobsDbContext dbContext,
        IServiceProvider services)
    {
        await CotJobRegistration.RegisterAsync(scheduler, dbContext, services);
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

    private static void AddJobs(IServiceCollection services, IConfiguration configuration)
    {
        CotJobRegistration.AddServices(services, configuration);
        CurrencyStrengthJobRegistration.AddServices(services, configuration);
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
        AddJobs(builder.Services, builder.Configuration);

        using var host = builder.Build();

        await using var scope = host.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BackgroundJobsDbContext>();
        await dbContext.Database.MigrateAsync();
        await CotJobRegistration.EnsureInfrastructureAsync(scope.ServiceProvider);

        var scheduler = await QuartzSchedulerBuilder.CreateAsync(scope.ServiceProvider);
        await AddJob_HelloWorld(scheduler, dbContext);
        await AddJob_CurrencyStrength(scheduler, dbContext, host.Services);
        await AddJob_Cot(scheduler, dbContext, host.Services);
        await scheduler.Start();

        var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStopping.Register(() =>
        {
            scheduler.Shutdown(waitForJobsToComplete: true).GetAwaiter().GetResult();
        });

        await host.RunAsync();
    }
}
