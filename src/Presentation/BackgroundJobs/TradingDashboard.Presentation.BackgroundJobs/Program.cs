using Quartz;
using TradingDashboard.Presentation.BackgroundJobs.Scheduler;

namespace TradingDashboard.Presentation.BackgroundJobs;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var scheduler = await QuartzSchedulerBuilder.CreateAsync();
        await AddJob_HelloWorld(scheduler);
        await scheduler.Start();
        await Task.Delay(TimeSpan.FromSeconds(1));
        await scheduler.Shutdown(waitForJobsToComplete: true);
    }

    public static async Task AddJob_HelloWorld(IScheduler scheduler)
    {
        var job = JobBuilder.Create<HelloWorldJob>()
            .WithIdentity("hello-world-job")
            .Build();

        var trigger = TriggerBuilder.Create()
            .WithIdentity("hello-world-trigger")
            .StartNow()
            .Build();

        await BackgroundJobRegistrar.AddJobAsync(scheduler, job, trigger);
    }
}
