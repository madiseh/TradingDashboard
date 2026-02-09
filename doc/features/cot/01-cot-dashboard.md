# Feature: COT Dashboard (Initial Draft)

## Objective

Provide a Commitment of Traders (COT) dashboard inspired by Tradingster-style analytics, with our own architecture and implementation.

## In Scope

- Display COT data by market/instrument.
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

## Data Requirements

- Inputs: COT weekly reports.
- Outputs: Aggregated time-series per instrument/group.
- Storage: Relational DB with indexed time-series fields.

## API/UI Notes

- Presentation stack includes both `Web API` and `Blazor`.
- Blazor must call Web API endpoints and must not access storage directly.
- Keep API contracts versioned from day one.
- API response models should be designed for chart-friendly payloads.

## Dependencies

- Data ingestion scheduler.
- Charting library.
- Storage provider.

## Risks

- Data normalization inconsistencies.
- High chart query load without caching.

## Acceptance Criteria

1. User can select an instrument and see latest COT positioning.
2. User can view historical trend for at least 3 participant groups.
3. Blazor dashboard data is fetched from Web API endpoints only.
4. API returns consistent results with dashboard data.
