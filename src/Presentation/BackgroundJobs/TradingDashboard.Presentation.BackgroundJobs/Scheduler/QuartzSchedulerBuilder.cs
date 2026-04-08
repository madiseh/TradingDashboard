using Quartz;
using Quartz.Impl;
using TradingDashboard.Presentation.BackgroundJobs.Scheduler.Listeners;

namespace TradingDashboard.Presentation.BackgroundJobs.Scheduler;

public static class QuartzSchedulerBuilder
{
    public static async Task<IScheduler> CreateAsync(IServiceProvider services)
    {
        var schedulerFactory = new StdSchedulerFactory();
        var scheduler = await schedulerFactory.GetScheduler();
        var listener = ActivatorUtilities.CreateInstance<BackgroundJobExecutionListener>(services);
        scheduler.ListenerManager.AddJobListener(listener);
        return scheduler;
    }
}
