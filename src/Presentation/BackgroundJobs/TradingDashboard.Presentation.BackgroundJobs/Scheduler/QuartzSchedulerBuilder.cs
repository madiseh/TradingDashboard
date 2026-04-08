using Quartz;
using Quartz.Impl;

namespace TradingDashboard.Presentation.BackgroundJobs.Scheduler;

public static class QuartzSchedulerBuilder
{
    public static async Task<IScheduler> CreateAsync()
    {
        var schedulerFactory = new StdSchedulerFactory();
        return await schedulerFactory.GetScheduler();
    }
}
