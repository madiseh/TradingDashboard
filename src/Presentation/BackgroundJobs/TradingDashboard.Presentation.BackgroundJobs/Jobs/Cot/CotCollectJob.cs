using Quartz;

namespace TradingDashboard.Presentation.BackgroundJobs.Jobs.Cot;

[DisallowConcurrentExecution]
public sealed class CotCollectJob : IJob
{
    private readonly CotCollectionRunner _runner;
    private readonly ILogger<CotCollectJob> _logger;

    public CotCollectJob(
        CotCollectionRunner runner,
        ILogger<CotCollectJob> logger)
    {
        _runner = runner;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting COT collection job.");

        var result = await _runner.RunOnceAsync(context.CancellationToken);

        Console.WriteLine(
            $"Cot job executed. SavedFiles={result.SavedFileCount}, SuccessfulFetches={result.SuccessfulFetchCount}");
    }
}
