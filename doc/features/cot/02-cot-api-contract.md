# COT API Contract (v1)

## Purpose

Define stable Web API contracts for the COT dashboard.
Blazor UI consumes these endpoints and does not access storage directly.

## Base

- Base URL: `/api/v1/cot`
- Models: `Cot`, `CotLegacy`
- Date format: `YYYY-MM-DD` (ISO)
- All endpoints return `application/json`

## Shared Enums

### ReportModel

- `Cot`
- `CotLegacy`

### ParticipantGroup

`CotLegacy`:
- `NonCommercial`
- `Commercial`
- `NonReportable`

`Cot`:
- `DealerIntermediary`
- `AssetManagerInstitutional`
- `LeveragedFunds`
- `OtherReportables`
- `NonreportablePositions`

## Common DTOs

### InstrumentDto

```json
{
  "instrumentId": "aud-cme-futures",
  "name": "Australian Dollar",
  "exchange": "CME",
  "cftcCode": "232741",
  "availableModels": ["Cot", "CotLegacy"]
}
```

### PositionValueDto

```json
{
  "positions": 33794,
  "openInterestPercent": 13.3,
  "tradersCount": 7,
  "weeklyChange": -2573
}
```

### GroupSnapshotDto

```json
{
  "group": "DealerIntermediary",
  "long": { "positions": 33794, "openInterestPercent": 13.3, "tradersCount": 7, "weeklyChange": -2573 },
  "short": { "positions": 108417, "openInterestPercent": 42.6, "tradersCount": 12, "weeklyChange": 15827 },
  "spread": { "positions": 181, "openInterestPercent": 0.1, "tradersCount": 0, "weeklyChange": 18 },
  "netPosition": -74623
}
```

### ReportHeaderDto

```json
{
  "instrumentId": "aud-cme-futures",
  "instrumentName": "Australian Dollar",
  "exchange": "CME",
  "cftcCode": "232741",
  "model": "Cot",
  "reportDate": "2026-02-03",
  "openInterest": 254240,
  "openInterestWeeklyChange": 2185,
  "totalTraders": 139
}
```

## Endpoints

### 1) Get Instruments

- `GET /api/v1/cot/instruments`

Query:
- `q` (optional): search by name/code
- `model` (optional): filter instruments that support this model

Response `200`:

```json
{
  "items": [
    {
      "instrumentId": "aud-cme-futures",
      "name": "Australian Dollar",
      "exchange": "CME",
      "cftcCode": "232741",
      "availableModels": ["Cot", "CotLegacy"]
    }
  ]
}
```

### 2) Get Supported Models

- `GET /api/v1/cot/models`

Response `200`:

```json
{
  "items": [
    {
      "model": "CotLegacy",
      "groups": ["NonCommercial", "Commercial", "NonReportable"]
    },
    {
      "model": "Cot",
      "groups": ["DealerIntermediary", "AssetManagerInstitutional", "LeveragedFunds", "OtherReportables", "NonreportablePositions"]
    }
  ]
}
```

### 3) Get Available Dates

- `GET /api/v1/cot/instruments/{instrumentId}/available-dates?model=Cot`

Purpose:
- Power Blazor date picker.
- Only returned dates are selectable.

Response `200`:

```json
{
  "instrumentId": "aud-cme-futures",
  "model": "Cot",
  "latestDate": "2026-02-03",
  "minDate": "2010-01-05",
  "maxDate": "2026-02-03",
  "availableDates": ["2026-02-03", "2026-01-27", "2026-01-20"]
}
```

### 4) Get Latest Snapshot

- `GET /api/v1/cot/instruments/{instrumentId}/latest?model=CotLegacy`

Response `200`:

```json
{
  "header": {
    "instrumentId": "aud-cme-futures",
    "instrumentName": "Australian Dollar",
    "exchange": "CME",
    "cftcCode": "232741",
    "model": "CotLegacy",
    "reportDate": "2026-02-03",
    "openInterest": 254240,
    "openInterestWeeklyChange": 2185,
    "totalTraders": 139
  },
  "groups": [
    {
      "group": "NonCommercial",
      "long": { "positions": 118751, "openInterestPercent": 46.7, "tradersCount": 55, "weeklyChange": 8945 },
      "short": { "positions": 92633, "openInterestPercent": 36.4, "tradersCount": 27, "weeklyChange": -10027 },
      "spread": { "positions": 1477, "openInterestPercent": 0.6, "tradersCount": 12, "weeklyChange": -116 },
      "netPosition": 26118
    }
  ]
}
```

### 5) Get Series

- `GET /api/v1/cot/instruments/{instrumentId}/series?model=Cot&group=LeveragedFunds&from=2025-01-07&to=2026-02-03`

Response `200`:

```json
{
  "instrumentId": "aud-cme-futures",
  "model": "Cot",
  "group": "LeveragedFunds",
  "items": [
    {
      "reportDate": "2026-02-03",
      "long": 98448,
      "short": 39039,
      "spread": 1364,
      "netPosition": 59409,
      "openInterest": 254240
    }
  ]
}
```

### 6) Get Overview (Dashboard Initial Load)

- `GET /api/v1/cot/instruments/{instrumentId}/overview?model=Cot&from=2025-08-05&to=2026-02-03`

Response `200`:

```json
{
  "header": {
    "instrumentId": "aud-cme-futures",
    "instrumentName": "Australian Dollar",
    "exchange": "CME",
    "cftcCode": "232741",
    "model": "Cot",
    "reportDate": "2026-02-03",
    "openInterest": 254240,
    "openInterestWeeklyChange": 2185,
    "totalTraders": 139
  },
  "latestGroups": [],
  "trendSummary": [
    { "group": "DealerIntermediary", "latestNetPosition": -74623, "weeklyNetChange": -18400 },
    { "group": "AssetManagerInstitutional", "latestNetPosition": -18802, "weeklyNetChange": 547 }
  ],
  "highlights": {
    "mostBullishGroup": "LeveragedFunds",
    "mostBearishGroup": "DealerIntermediary"
  }
}
```

### 7) Get Report By Date

- `GET /api/v1/cot/reports/{reportDate}?model=CotLegacy`

Response `200`:

```json
{
  "reportDate": "2026-02-03",
  "model": "CotLegacy",
  "items": [
    {
      "instrumentId": "aud-cme-futures",
      "openInterest": 254240,
      "totalTraders": 139
    }
  ]
}
```

## Validation Rules

1. `model` is required for model-sensitive endpoints.
2. `group` must belong to selected `model`.
3. `from <= to`.
4. `reportDate` must be an available official COT report date.
5. If `from` or `to` is not in available dates, API normalizes range to nearest lower available dates and returns effective range in response metadata.

## Error Contract

Error format:

```json
{
  "error": {
    "code": "INVALID_DATE",
    "message": "Requested reportDate is not available for selected instrument/model.",
    "details": {
      "instrumentId": "aud-cme-futures",
      "model": "Cot",
      "requestedDate": "2026-02-01"
    }
  }
}
```

Common codes:
- `INVALID_MODEL`
- `INVALID_GROUP`
- `INVALID_DATE`
- `INVALID_RANGE`
- `INSTRUMENT_NOT_FOUND`

## Blazor Consumption Notes

1. On instrument/model change:
- call `available-dates`
- set date picker enabled dates
- pick `latestDate` by default

2. Initial page load:
- call `overview`
- render header, table, and summary cards

3. Date or range change:
- call `latest` or `series` depending on interaction

4. Do not query database directly from Blazor.
