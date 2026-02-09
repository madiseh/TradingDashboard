# Project Structure (Planned)

```text
TradingDashboard/
|-- TradingDashboard.sln
|-- .gitignore
|-- src/
|   |-- Domain/
|   |   |-- TradingDashboard.Domain.Abstraction/
|   |   `-- TradingDashboard.Domain/
|   |-- Application/
|   |   `-- TradingDashboard.Application/
|   |-- Infrastructure/
|   |   |-- Implementation/
|   |   |   `-- TradingDashboard.Infrastructure/
|   |   `-- Persistence/
|   |       |-- TradingDashboard.Infrastructure.Persistence.Common/
|   |       `-- TradingDashboard.Infrastructure.Persistence.Implementation/
|   `-- Presentation/
|       |-- Common/
|       |   `-- TradingDashboard.Presentation.Common/
|       |-- TradingDashboard.Presentation.Api/
|       `-- TradingDashboard.Presentation.Blazor/
|-- test/
|   |-- TradingDashboard.UnitTests/
|   `-- TradingDashboard.IntegrationTests/

## Notes

- Project folders under `src` and `test` are placeholders for now.
- Documentation folders under `doc` are organized into `governance`, `features`, and `uiComponent`.
- No implementation project is created yet.
- Naming follows `TradingDashboard.<Layer>` convention.
- Blazor UI consumes data from `TradingDashboard.Presentation.Api` only.
- Blazor presentation stack uses `MudBlazor`.
- `TradingDashboard.Domain.Abstraction` is a dedicated project for shared base domain types.
- Persistence is split into `Infrastructure.Persistence.Common` and `Infrastructure.Persistence.Implementation`.
- Seed data belongs to `Infrastructure.Persistence.Implementation/Seed` and should be provided through static seed classes.
