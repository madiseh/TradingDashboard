using TradingDashboard.Domain.Common.Common;

namespace TradingDashboard.Domain.Entities;

public sealed class CotCollectionJobState : BaseAuditableEntity<string>
{
    public DateTime? LastStartedAtUtc { get; private set; }
    public DateTime? LastCompletedAtUtc { get; private set; }
    public DateTime? LastSucceededAtUtc { get; private set; }
    public int LastSavedFileCount { get; private set; }
    public int LastFetchedProfileCount { get; private set; }
    public string? LastError { get; private set; }

    public CotCollectionJobState(string id, DateTime createdAtUtc)
        : base(
            string.IsNullOrWhiteSpace(id) ? throw new ArgumentException("State id cannot be empty.", nameof(id)) : id.Trim(),
            createdAtUtc)
    {
    }

    private CotCollectionJobState() : base(string.Empty, DateTime.UtcNow)
    {
    }

    public void RecordStart(DateTime startedAtUtc)
    {
        LastStartedAtUtc = startedAtUtc;
        LastError = null;
        MarkUpdated(startedAtUtc);
    }

    public void RecordSuccess(DateTime completedAtUtc, int savedFileCount, int fetchedProfileCount)
    {
        if (savedFileCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(savedFileCount), "Saved file count cannot be negative.");
        }

        if (fetchedProfileCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fetchedProfileCount), "Fetched profile count cannot be negative.");
        }

        LastCompletedAtUtc = completedAtUtc;
        LastSucceededAtUtc = completedAtUtc;
        LastSavedFileCount = savedFileCount;
        LastFetchedProfileCount = fetchedProfileCount;
        LastError = null;
        MarkUpdated(completedAtUtc);
    }

    public void RecordFailure(DateTime completedAtUtc, string errorMessage, int savedFileCount, int fetchedProfileCount)
    {
        if (savedFileCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(savedFileCount), "Saved file count cannot be negative.");
        }

        if (fetchedProfileCount < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fetchedProfileCount), "Fetched profile count cannot be negative.");
        }

        LastCompletedAtUtc = completedAtUtc;
        LastSavedFileCount = savedFileCount;
        LastFetchedProfileCount = fetchedProfileCount;
        LastError = string.IsNullOrWhiteSpace(errorMessage)
            ? "Unknown collector error."
            : errorMessage.Trim();
        MarkUpdated(completedAtUtc);
    }
}
