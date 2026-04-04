# Feature: Currency Strength Dashboard

## Objective

Provide a currency-strength collector and dashboard that continuously captures predefined time-range datasets and renders them as a multi-currency line chart in Blazor.

## In Scope

- Poll the configured FX Chart endpoints every 1 minute.
- Persist the raw payloads per timeframe under feature-owned storage folders.
- Expose read-only Web API endpoints for timeframe metadata and latest collected snapshot.
- Render a dedicated Blazor page with timeframe switching and a multi-series currency chart.

## Out of Scope

- Trading signals or order execution.
- Derived indicators outside the collected source payload.
- User-defined custom URLs in phase 1.

## User Stories

- As a trader, I want to switch between `10m`, `30m`, `1H`, `1W`, and long-term views to compare relative currency strength.
- As an analyst, I want the collector to keep checking the source every minute so the dashboard always reflects the latest published curve.

## Functional Requirements

1. Collector must request all configured currency-strength URLs every minute.
2. Collector must store each timeframe under its own folder and keep a `latest.json` file for fast reads.
3. Collector must also store timestamped snapshots for audit/debugging.
4. API must return the supported timeframe list in display order.
5. API must return the latest collected multi-currency series for a selected timeframe.
6. Blazor must render one dedicated page under the main project menu.
7. Blazor must call the feature API only and must not read storage directly.

## Non-Functional Requirements

- Collector should continue processing other timeframes even if one source URL fails.
- API should stay read-only and lightweight by reading the stored `latest.json` files.
- UI should render smoothly on desktop and mobile.
- Storage layout should stay simple enough for future extraction into a separate service.

## Data Requirements

- Input: JSON array of currency series shaped as `{ key, values }`.
- Output: Chart-ready response with timeframe metadata, collection timestamp, and per-currency time-series points.
- Storage: filesystem folders grouped by timeframe.

## Acceptance Criteria

1. A new feature folder exists under `src/Features/CurrencyStrength`.
2. Collector polls all requested URLs every 1 minute.
3. Raw payloads are saved to disk under a dedicated currency-strength storage root.
4. API returns a valid latest snapshot for each configured timeframe.
5. Blazor shows a dedicated Currency Strength page and chart.
6. The project menu contains a direct link to the new page.
