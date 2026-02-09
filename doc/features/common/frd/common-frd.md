# Common Market Directory FRD

## Goal

Define requirements for shared market-directory APIs and Home listing behavior.

## Scope

- Shared listing APIs.
- Home page grouped directory UI.
- Instrument-level navigation actions.

## UI Requirements

1. Show sections by market type (initially: `Currencies`, `Grains`, `Cryptocurrencies`).
2. Render instruments in each category as table/list rows.
3. Each row must include:
- instrument name
- `COT Legacy` action
- `COT` action
4. Layout should remain usable on desktop and mobile.

## Navigation Requirements

1. `COT Legacy` action route:
- `/cot/{instrumentSlug}?model=CotLegacy`
2. `COT` action route:
- `/cot/{instrumentSlug}?model=Cot`
3. Route must preserve selected model on page load and refresh.

## Data Requirements

1. Home page loads grouped data from:
- `GET /api/v1/common/markets-directory`
2. Type-specific lists are available from:
- `GET /api/v1/common/markets?type={type}`
3. Data shape includes:
- `type`
- `instrumentId`
- `slug`
- `name`
- `availableModels`

## Validation Rules

1. Show link only for models present in `availableModels`.
2. If no items exist for a category, section should be hidden.
3. If API fails, show non-blocking error message and retry action.

## Acceptance Criteria

1. User sees grouped sections with instrument rows.
2. User can navigate to both models from each eligible row.
3. Routes open the selected instrument with correct model query.
4. No hardcoded instrument list is used in UI.
