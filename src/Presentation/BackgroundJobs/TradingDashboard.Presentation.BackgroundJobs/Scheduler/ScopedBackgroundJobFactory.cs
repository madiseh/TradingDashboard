using System.Collections.Concurrent;
using Quartz;
using Quartz.Spi;

namespace TradingDashboard.Presentation.BackgroundJobs.Scheduler;

public sealed class ScopedBackgroundJobFactory : IJobFactory
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ConcurrentDictionary<IJob, IServiceScope> _scopes = new();

    public ScopedBackgroundJobFactory(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
    {
        var scope = _scopeFactory.CreateScope();

        try
        {
            var job = (IJob)ActivatorUtilities.CreateInstance(scope.ServiceProvider, bundle.JobDetail.JobType);
            _scopes[job] = scope;
            return job;
        }
        catch
        {
            scope.Dispose();
            throw;
        }
    }

    public void ReturnJob(IJob job)
    {
        if (job is IDisposable disposable)
        {
            disposable.Dispose();
        }

        if (_scopes.TryRemove(job, out var scope))
        {
            scope.Dispose();
        }
    }
}
