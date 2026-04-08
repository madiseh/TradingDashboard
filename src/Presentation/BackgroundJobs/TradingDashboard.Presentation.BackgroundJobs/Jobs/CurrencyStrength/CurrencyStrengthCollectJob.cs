using Quartz;

namespace TradingDashboard.Presentation.BackgroundJobs.Jobs.CurrencyStrength;

public sealed class CurrencyStrengthCollectJob : IJob
{
    private readonly CurrencyStrengthCollectionRunner _runner;
    private readonly ILogger<CurrencyStrengthCollectJob> _logger;

    public CurrencyStrengthCollectJob(
        CurrencyStrengthCollectionRunner runner,
        ILogger<CurrencyStrengthCollectJob> logger)
    {
        _runner = runner;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting currency strength collection job.");

        var result = await _runner.RunOnceAsync(context.CancellationToken);

        Console.WriteLine(
            $"CurrencyStrength job executed. SavedFiles={result.SavedFileCount}, SuccessfulFetches={result.SuccessfulFetchCount}");
    }
}
