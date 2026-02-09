# COT Use Cases

## Purpose

Define application-level use cases for COT dashboard and API.

## Read Use Cases

### Get Instruments

- Input: `q?`, `model?`
- Output: list of instruments with available models.
- API mapping: `GET /api/v1/cot/instruments`

### Get Supported Models

- Input: none
- Output: models and allowed groups.
- API mapping: `GET /api/v1/cot/models`

### Get Available Dates

- Input: `instrumentId`, `model`
- Output: `availableDates`, `latestDate`, `minDate`, `maxDate`.
- API mapping: `GET /api/v1/cot/instruments/{instrumentId}/available-dates`

### Get Latest Snapshot

- Input: `instrumentId`, `model`
- Output: header + grouped latest metrics.
- API mapping: `GET /api/v1/cot/instruments/{instrumentId}/latest`

### Get Series

- Input: `instrumentId`, `model`, `group`, `from`, `to`
- Output: time-series rows for charts.
- API mapping: `GET /api/v1/cot/instruments/{instrumentId}/series`

### Get Overview

- Input: `instrumentId`, `model`, `from`, `to`
- Output: dashboard initial payload (header + summary + highlights).
- API mapping: `GET /api/v1/cot/instruments/{instrumentId}/overview`

### Get Report By Date

- Input: `reportDate`, `model`
- Output: instrument-level snapshot list for that date.
- API mapping: `GET /api/v1/cot/reports/{reportDate}`

## Write/Processing Use Cases

### Ingest Weekly Report

- Input: source file/records, `reportDate`, `model`
- Behavior: normalize -> validate -> upsert report and positions.
- Output: ingestion summary and validation issues.

### Rebuild Aggregates

- Input: `instrumentId?`, `model?`, date window.
- Behavior: recompute read models and caches.
- Output: rebuild result counters.

## Validation Rules (Application Layer)

1. Reject invalid `model`.
2. Reject invalid `group` for model.
3. Reject date not in available report dates.
4. Reject `from > to`.
5. Return consistent error contract from API layer.

## Blazor Flow

1. Load `instruments` and `models`.
2. On selection change, load `available-dates`.
3. Select `latestDate` by default.
4. Load `overview`.
5. On chart interactions, call `series` with selected range/group.
