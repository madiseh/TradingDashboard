using Quartz;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Context;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Entities;

namespace TradingDashboard.Presentation.BackgroundJobs.Scheduler.Listeners;

public sealed class BackgroundJobExecutionListener : IJobListener
{
    private readonly IServiceScopeFactory _scopeFactory;

    public BackgroundJobExecutionListener(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public string Name => nameof(BackgroundJobExecutionListener);

    public Task JobToBeExecuted(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task JobExecutionVetoed(IJobExecutionContext context, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public async Task JobWasExecuted(
        IJobExecutionContext context,
        JobExecutionException? jobException,
        CancellationToken cancellationToken = default)
    {
        if (jobException is null)
        {
            Console.WriteLine($"Job '{context.JobDetail.Key}' executed successfully.");
        }
        else
        {
            var rootException = jobException.InnerException ?? jobException;
            Console.WriteLine($"Job '{context.JobDetail.Key}' failed: {rootException.Message}");
        }

        await using var scope = _scopeFactory.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<BackgroundJobsDbContext>();

        dbContext.BackgroundJobExecutionLogs.Add(
            BackgroundJobExecutionLog.Create(context, jobException));

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
