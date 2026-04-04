# Currency Strength Service Contract

## Collector Responsibilities

- Poll all configured timeframe endpoints every 1 minute.
- Save the newest payload to `latest.json`.
- Save timestamped snapshot files for traceability.
- Expose operational status and manual run endpoints.

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
