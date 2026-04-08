using Quartz;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Context;
using TradingDashboard.Presentation.BackgroundJobs.Scheduler;

namespace TradingDashboard.Presentation.BackgroundJobs.Jobs.HelloWorld;

public static class HelloWorldJobRegistration
{
    public static async Task RegisterAsync(IScheduler scheduler, BackgroundJobsDbContext dbContext)
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
}
