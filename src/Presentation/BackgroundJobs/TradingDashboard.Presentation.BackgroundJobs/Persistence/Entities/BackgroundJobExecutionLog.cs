using Quartz;

namespace TradingDashboard.Presentation.BackgroundJobs.Persistence.Entities;

public sealed class BackgroundJobExecutionLog
{
    private BackgroundJobExecutionLog()
    {
    }

    private BackgroundJobExecutionLog(
        string jobName,
        string jobGroup,
        string triggerName,
        string triggerGroup,
        bool isSuccessful,
        string? exceptionType,
        string? errorMessage,
        DateTime executedAtUtc)
    {
        JobName = jobName;
        JobGroup = jobGroup;
        TriggerName = triggerName;
        TriggerGroup = triggerGroup;
        IsSuccessful = isSuccessful;
        ExceptionType = exceptionType;
        ErrorMessage = errorMessage;
        ExecutedAtUtc = executedAtUtc;
    }

    public long Id { get; private set; }
    public string JobName { get; private set; } = null!;
    public string JobGroup { get; private set; } = null!;
    public string TriggerName { get; private set; } = null!;
    public string TriggerGroup { get; private set; } = null!;
    public bool IsSuccessful { get; private set; }
    public string? ExceptionType { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime ExecutedAtUtc { get; private set; }

    public static BackgroundJobExecutionLog Create(IJobExecutionContext context, JobExecutionException? jobException)
    {
        var rootException = jobException?.InnerException ?? jobException;

        return new BackgroundJobExecutionLog(
            context.JobDetail.Key.Name,
            context.JobDetail.Key.Group,
            context.Trigger.Key.Name,
            context.Trigger.Key.Group,
            jobException is null,
            rootException?.GetType().FullName,
            rootException?.ToString(),
            context.FireTimeUtc.UtcDateTime);
    }
}
