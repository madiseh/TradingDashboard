# Project Structure (Planned)

```text
TradingDashboard/
|-- TradingDashboard.sln
|-- .gitignore
|-- src/
|   |-- TradingDashboard.Domain/
|   |-- TradingDashboard.Application/
|   |-- TradingDashboard.Infrastructure/
|   |-- TradingDashboard.Presentation.Api/
|   `-- TradingDashboard.Presentation.Blazor/
|-- test/
|   |-- TradingDashboard.UnitTests/
|   `-- TradingDashboard.IntegrationTests/
`-- doc/
    |-- README.md
    |-- governance/
    |   |-- README.md
    |   `-- architecture-principles.md
    `-- features/
        |-- README.md
        |-- _template.md
        `-- cot/
            `-- 01-cot-dashboard.md
```

## Notes

- Project folders under `src` and `test` are placeholders for now.
- No implementation project is created yet.
- Naming follows `TradingDashboard.<Layer>` convention.
- Blazor UI consumes data from `TradingDashboard.Presentation.Api` only.
