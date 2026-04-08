using Quartz;

namespace TradingDashboard.Presentation.BackgroundJobs.Scheduler;

public static class BackgroundJobRegistrar
{
    public static async Task AddJobAsync(
        IScheduler scheduler,
        IJobDetail job,
        ITrigger trigger)
    {
        await scheduler.ScheduleJob(job, trigger);
    }
}
