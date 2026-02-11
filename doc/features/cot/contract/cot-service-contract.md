# COT Service Contract

## Purpose

Define service-level interfaces and responsibilities for COT module.

## Interfaces

### `ICotReportRepository`

- Responsibility: provides COT report persistence access.

Methods:
- `GetLatestByInstrument`
- `GetRange`

### `ICotPositionRepository`

- Responsibility: provides COT position snapshots and series access.

Methods:
- `GetSeries`
- `GetLatestSnapshot`

### `IInstrumentRepository`

- Responsibility: provides instrument lookup data used by COT flows.

Methods:
- `GetAll`
- `GetByCode`
- `GetBySlug`
- `GetByGroup`

## Notes

- Interface details are owned by module contracts and must be documented here.
- Every interface and method in code must include XML `summary`.
