# Currency Strength Service Contract

## Collector Responsibilities

- Run as a scheduled Quartz job inside `TradingDashboard.Presentation.BackgroundJobs`.
- Poll all configured timeframe endpoints every 1 minute.
- Save the newest payload to `latest.json`.
- Save timestamped snapshot files for traceability.

## Storage Layout

```text
storage/currency-strength/
  30min/
    latest.json
    snapshots/
      2026-04-03/
        11-40-00.json
  1week/
    latest.json
```
