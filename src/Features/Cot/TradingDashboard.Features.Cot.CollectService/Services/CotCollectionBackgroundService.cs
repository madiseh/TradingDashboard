using TradingDashboard.Features.Cot.CollectService.Abstractions;
using TradingDashboard.Features.Cot.CollectService.Options;

namespace TradingDashboard.Features.Cot.CollectService.Services;

public sealed class CotCollectionBackgroundService : BackgroundService
{
    private readonly ICotCollectionCoordinator _coordinator;
    private readonly ILogger<CotCollectionBackgroundService> _logger;
    private readonly CotCollectorOptions _options;

    public CotCollectionBackgroundService(
        ICotCollectionCoordinator coordinator,
        IOptions<CotCollectorOptions> options,
        ILogger<CotCollectionBackgroundService> logger)
    {
        _coordinator = coordinator;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_options.IntervalMinutes <= 0)
        {
            throw new InvalidOperationException("CotCollector:IntervalMinutes must be greater than zero.");
        }

        if (_options.RunOnStartup)
        {
            try
            {
                await _coordinator.RunOnceAsync("startup", stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Initial collector run failed.");
            }
        }

        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(_options.IntervalMinutes));
        _coordinator.SetNextScheduledRun(DateTime.UtcNow.AddMinutes(_options.IntervalMinutes));

        while (!stoppingToken.IsCancellationRequested)
        {
            var hasTicked = await timer.WaitForNextTickAsync(stoppingToken);
            if (!hasTicked)
            {
                break;
            }

            _coordinator.SetNextScheduledRun(DateTime.UtcNow.AddMinutes(_options.IntervalMinutes));

            try
            {
                await _coordinator.RunOnceAsync("schedule", stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Scheduled collector run failed.");
            }
        }
    }
}
