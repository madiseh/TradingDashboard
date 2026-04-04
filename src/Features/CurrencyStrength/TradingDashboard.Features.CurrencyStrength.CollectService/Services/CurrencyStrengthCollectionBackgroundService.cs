using TradingDashboard.Features.CurrencyStrength.CollectService.Abstractions;
using TradingDashboard.Features.CurrencyStrength.CollectService.Options;

namespace TradingDashboard.Features.CurrencyStrength.CollectService.Services;

public sealed class CurrencyStrengthCollectionBackgroundService : BackgroundService
{
    private readonly ICurrencyStrengthCollectionCoordinator _coordinator;
    private readonly ILogger<CurrencyStrengthCollectionBackgroundService> _logger;
    private readonly CurrencyStrengthCollectorOptions _options;

    public CurrencyStrengthCollectionBackgroundService(
        ICurrencyStrengthCollectionCoordinator coordinator,
        IOptions<CurrencyStrengthCollectorOptions> options,
        ILogger<CurrencyStrengthCollectionBackgroundService> logger)
    {
        _coordinator = coordinator;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_options.IntervalMinutes <= 0)
        {
            throw new InvalidOperationException("CurrencyStrengthCollector:IntervalMinutes must be greater than zero.");
        }

        if (_options.RunOnStartup)
        {
            try
            {
                await _coordinator.RunOnceAsync("startup", stoppingToken);
            }
            catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogError(ex, "Initial currency strength collector run failed.");
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
                _logger.LogError(ex, "Scheduled currency strength collector run failed.");
            }
        }
    }
}
