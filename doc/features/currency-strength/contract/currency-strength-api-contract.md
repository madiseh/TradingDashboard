# Currency Strength API Contract

## Base

- Base URL: `/api/v1/currency-strength`
- Content type: `application/json`

## Endpoints

### Get Timeframes

- `GET /api/v1/currency-strength/timeframes`

### Get Latest Snapshot

- `GET /api/v1/currency-strength/timeframes/{timeframeKey}/latest`

Response shape:

```json
{
  "timeframeKey": "30min",
  "timeframeLabel": "30m",
  "description": "Last 30 minutes",
  "collectedAtUtc": "2026-04-03T11:40:00Z",
  "lastPointAtUtc": "2026-04-03T11:39:00Z",
  "series": [
    {
      "currencyCode": "EUR",
      "points": [
        {
          "timestamp": 1775218740000,
          "value": 1.42
        }
      ]
    }
  ]
}
```
