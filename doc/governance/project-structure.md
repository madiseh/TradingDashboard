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
|   |       |-- TradingDashboard.Infrastructure.Persistence.Implementation/
|   |       `-- TradingDashboard.Infrastructure.Persistence.SimulatedData/
|   `-- Presentation/
|       |-- Common/
|       |   `-- TradingDashboard.Presentation.Common/
|       |-- TradingDashboard.Presentation.Api/
|       |-- TradingDashboard.Presentation.Blazor/
|       `-- TradingDashboard.Presentation.MauiHybrid/
|-- test/
|   |-- TradingDashboard.UnitTests/
|   `-- TradingDashboard.IntegrationTests/

## Notes

- Project folders under `src` and `test` are placeholders for now.
- Documentation folders under `doc` are organized into `governance`, `features`, and `uiComponent`.
- No implementation project is created yet.
- Naming follows `TradingDashboard.<Layer>` convention.
- Blazor UI consumes data from `TradingDashboard.Presentation.Api` only.
- MAUI Hybrid host consumes the same API surface through named `HttpClient` registrations.
- Blazor presentation stack uses `MudBlazor`.
- MAUI Hybrid presentation stack uses `BlazorWebView` with `MudBlazor`.
- Blazor modules/pages must keep view models in sibling files named `<Component>.ViewModels.cs` next to `<Component>.razor` and `<Component>.razor.cs`.
- `TradingDashboard.Domain.Abstraction` is a dedicated project for shared base domain types.
- Persistence is split into `Infrastructure.Persistence.Common` and `Infrastructure.Persistence.Implementation`.
- `Infrastructure.Persistence.SimulatedData` contains fake/json-backed repositories for development and local simulation.
- Seed data belongs to `Infrastructure.Persistence.Implementation/Seed` and should be provided through static seed classes.
