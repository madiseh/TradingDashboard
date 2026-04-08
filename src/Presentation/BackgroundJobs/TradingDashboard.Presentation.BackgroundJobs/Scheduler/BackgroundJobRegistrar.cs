using Microsoft.EntityFrameworkCore;
using Quartz;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Context;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Entities;

namespace TradingDashboard.Presentation.BackgroundJobs.Scheduler;

public static class BackgroundJobRegistrar
{
    public static async Task AddJobAsync(
        IScheduler scheduler,
        BackgroundJobsDbContext dbContext,
        IJobDetail job,
        ITrigger trigger,
        CancellationToken cancellationToken = default)
    {
        var registeredJob = await dbContext.BackgroundJobs
            .SingleOrDefaultAsync(
                x => x.JobName == job.Key.Name && x.JobGroup == job.Key.Group,
                cancellationToken);

        if (registeredJob?.IsCancelled() == true)
        {
            Console.WriteLine($"Job '{job.Key}' is cancelled and will not be scheduled.");
            return;
        }

        await scheduler.ScheduleJob(job, trigger, cancellationToken);

        var jobType = job.JobType.FullName ?? job.JobType.Name;

        if (registeredJob is null)
        {
            dbContext.BackgroundJobs.Add(BackgroundJobRecord.Create(job.Key, trigger.Key, jobType));
        }
        else
        {
            registeredJob.UpdateRegistration(trigger.Key, jobType);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
