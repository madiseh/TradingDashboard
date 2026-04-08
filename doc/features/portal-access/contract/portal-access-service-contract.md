# Portal Access Service Contract

## UI Session Responsibilities

- Store authenticated token metadata.
- Store authenticated user role.
- Resolve post-login destination by role.
- Guard `Admin` and `TradingPanel` routes in Blazor.

## Routing Rules

- `SuperAdmin` -> `Admin`
- `Analyzer` -> `TradingPanel`
- `Guest` -> `TradingPanel`
