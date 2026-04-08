# Feature: COT Dashboard (Initial Draft)

## Objective

Provide a Commitment of Traders (COT) dashboard inspired by Tradingster-style analytics, with our own architecture and implementation.

## In Scope

- Display COT data by market/instrument.
- Support both report models: `COT` and `COT Legacy`.
- Show net positioning trends over time.
- Allow filtering by participant groups.
- Provide charts and tabular views.

## Out of Scope

- Automated trading execution.
- Portfolio management.
- Paid data vendor integrations in phase 1.

## User Stories

- As a trader, I want to compare commercial vs non-commercial positioning trends.
- As an analyst, I want historical COT snapshots to inspect market sentiment shifts.

## Functional Requirements

1. Import and normalize COT source data.
2. Query data by instrument and date range.
3. Expose Web API endpoints for dashboard consumption.
4. Build a high-end Blazor dashboard that requests data only through the Web API.
5. Render key COT charts and tabular analytics in Blazor views.
6. Keep API contracts reusable for future external consumers.

## Non-Functional Requirements

- Performance: Dashboard initial load under 2 seconds for cached queries.
- Security: Read-only public views in phase 1; admin endpoints protected.
- Observability: Track ingestion job status and query latency.
- UX: Blazor dashboard should provide responsive and smooth chart interactions on desktop.
- UX: Calendar/date picker must disable non-report dates and allow selection only on valid report dates.

## Data Requirements

- Inputs: COT weekly reports.
- Outputs: Aggregated time-series per instrument/group.
- Storage: Relational DB with indexed time-series fields.

## Report Models

The dashboard must support two distinct models with different row/category structures.

### Model 1: `COT Legacy`

- Group set:
- `NonCommercial`
- `Commercial`
- `NonReportable`
- Summary columns include total long/short and open interest context.
- Spread values may appear for reportable categories.

### Model 2: `COT` (Disaggregated)

- Group set:
- `DealerIntermediary`
- `AssetManagerInstitutional`
- `LeveragedFunds`
- `OtherReportables`
- `NonreportablePositions`
- Each group can contain `Long`, `Short`, and `Spread` dimensions with:
- `positions`
- `openInterestPercent`
- `tradersCount`

## API/UI Notes

- Presentation stack includes both `Web API` and `Blazor`.
- Blazor must call Web API endpoints and must not access storage directly.
- Keep API contracts versioned from day one.
- API response models should be designed for chart-friendly payloads.

## Domain Layer Requirements

### Core Entities

- `CotReport`: One weekly report per market and report date; includes open interest and source metadata.
- `CotPosition`: Position values for one participant group in one report; includes long, short, spreading (if available), and net.
- `Instrument`: Canonical tradable market/instrument with source-to-internal code mapping.
- `ParticipantGroup`: Model-aware category enum. Allowed values depend on `ReportModel`.
- `ReportModel`: Report type enum (`Cot`, `CotLegacy`).

### Value Objects

- `ReportDate` (weekly date with UTC normalization).
- `PositionMetrics` (long, short, spreading, net).
- `ChangeMetrics` (weekly delta and percentage change).
- `ModelSpecificMetrics` (`openInterestPercent`, `tradersCount`).

### Domain Services

- `CotNormalizationService`: Converts raw source records into canonical domain entities.
- `CotAggregationService`: Produces trend-ready series and summary aggregates per instrument and group.

### Domain Rules

1. One `CotReport` per `Instrument` per `ReportDate`.
2. `NetPosition = Long - Short`.
3. No negative values for `Long`, `Short`, and `OpenInterest`.
4. Duplicate source rows for the same market/date/group are rejected during normalization.
5. A report is always tied to exactly one `ReportModel`.
6. Group validation is model-aware:
7. `CotLegacy` accepts only (`NonCommercial`, `Commercial`, `NonReportable`).
8. `Cot` accepts only disaggregated groups (`DealerIntermediary`, `AssetManagerInstitutional`, `LeveragedFunds`, `OtherReportables`, `NonreportablePositions`).
9. `ReportDate` must be an official COT `as-of` date (weekly cycle).
10. Date selection is restricted to available report dates in storage; arbitrary calendar dates are not allowed.

### Repository Contracts (Application-facing)

- `ICotReportRepository`: `GetLatestByInstrument(instrumentId)`, `GetRange(instrumentId, fromDate, toDate)`.
- `ICotPositionRepository`: `GetSeries(instrumentId, participantGroup, fromDate, toDate)`, `GetLatestSnapshot(instrumentId)`.
- `IInstrumentRepository`: `GetAll()`, `GetByCode(sourceCode)`.

## Web API Endpoints (v1)

Base route: `/api/v1/cot`

1. `GET /api/v1/cot/instruments`
- Returns available instruments for dashboard filters.

2. `GET /api/v1/cot/instruments/{instrumentId}/latest?model=CotLegacy`
- Returns latest snapshot for all participant groups in selected model.

3. `GET /api/v1/cot/instruments/{instrumentId}/series?model=Cot&from=YYYY-MM-DD&to=YYYY-MM-DD&group=LeveragedFunds`
- Returns trend series for one participant group in selected model.

4. `GET /api/v1/cot/instruments/{instrumentId}/overview?model=CotLegacy&from=YYYY-MM-DD&to=YYYY-MM-DD`
- Returns combined payload for dashboard initial load in selected model: latest snapshot, multi-group trend summaries, and key indicators.

5. `GET /api/v1/cot/reports/{reportDate}?model=Cot`
- Returns all instrument snapshots for a specific report week in selected model.

6. `GET /api/v1/cot/models`
- Returns supported models and valid group sets for each model.

7. `GET /api/v1/cot/instruments/{instrumentId}/available-dates?model=Cot`
- Returns selectable report dates for that instrument and model (used by Blazor date picker).

### API Response Contract Notes

- Use ISO date format (`YYYY-MM-DD`) in all responses.
- Include `model` field in every response (`Cot` or `CotLegacy`).
- Include both raw values and computed metrics (`netPosition`, `weeklyChange`).
- Return stable `instrumentId` and `group` keys for Blazor chart binding.
- Version DTOs under `v1` namespace to support future non-breaking evolution.
- If a requested date is not in available report dates, API returns `400 Bad Request` with validation details.

### Blazor Integration Notes

- Blazor pages call these API endpoints through typed HTTP clients.
- No direct database or infrastructure access from Blazor components.
- Dashboard pages use `instruments` for filters, `overview` for initial screen, and `series` for interactive chart range changes.
- Date picker must enable only dates returned by `available-dates` endpoint.
- Default selected date is latest available `as-of` date for the selected instrument/model.

## Dependencies

- Data ingestion scheduler.
- Charting library.
- Storage provider.

## Ingestion Runtime

- Raw COT collection runs in `TradingDashboard.Presentation.BackgroundJobs` as a Quartz job.
- The job owns startup backfill, incremental collection, COT database migration, and collector-state logging.

## Risks

- Data normalization inconsistencies.
- High chart query load without caching.

## Acceptance Criteria

1. User can select an instrument and see latest COT positioning.
2. User can view historical trend for at least 3 participant groups.
3. Blazor dashboard data is fetched from Web API endpoints only.
4. API returns consistent results with dashboard data.
