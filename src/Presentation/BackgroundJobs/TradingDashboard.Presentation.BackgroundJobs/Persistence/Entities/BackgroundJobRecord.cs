using Quartz;
using TradingDashboard.Presentation.BackgroundJobs.Persistence.Constants;

namespace TradingDashboard.Presentation.BackgroundJobs.Persistence.Entities;

public sealed class BackgroundJobRecord
{
    private BackgroundJobRecord()
    {
    }

    private BackgroundJobRecord(
        string id,
        string jobName,
        string jobGroup,
        string triggerName,
        string triggerGroup,
        string status,
        string jobType,
        DateTime createdAtUtc)
    {
        Id = id;
        JobName = jobName;
        JobGroup = jobGroup;
        TriggerName = triggerName;
        TriggerGroup = triggerGroup;
        Status = status;
        JobType = jobType;
        CreatedAtUtc = createdAtUtc;
        UpdatedAtUtc = createdAtUtc;
    }

    public string Id { get; private set; } = null!;
    public string JobName { get; private set; } = null!;
    public string JobGroup { get; private set; } = null!;
    public string TriggerName { get; private set; } = null!;
    public string TriggerGroup { get; private set; } = null!;
    public string Status { get; private set; } = null!;
    public string JobType { get; private set; } = null!;
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime UpdatedAtUtc { get; private set; }

    public static BackgroundJobRecord Create(JobKey jobKey, TriggerKey triggerKey, string jobType)
    {
        var now = DateTime.UtcNow;
        return new BackgroundJobRecord(
            Guid.NewGuid().ToString("N"),
            jobKey.Name,
            jobKey.Group,
            triggerKey.Name,
            triggerKey.Group,
            BackgroundJobStatuses.Running,
            jobType,
            now);
    }

    public void UpdateRegistration(TriggerKey triggerKey, string jobType)
    {
        TriggerName = triggerKey.Name;
        TriggerGroup = triggerKey.Group;
        JobType = jobType;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public bool IsCancelled()
    {
        return string.Equals(Status, BackgroundJobStatuses.Cancelled, StringComparison.OrdinalIgnoreCase);
    }
}
