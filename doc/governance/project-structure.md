# Project Structure (Planned)

```text
TradingDashboard/
|-- TradingDashboard.sln
|-- .gitignore
|-- src/
|   |-- TradingDashboard.Domain.Abstraction/
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
    |   |-- project-structure.md
    |   `-- architecture-principles.md
    `-- features/
        |-- README.md
        |-- _template.md
        `-- cot/
            |-- README.md
            |-- contract/
            |   `-- api-contract.md
            |-- domain/
            |   `-- domain-contract.md
            |-- frd/
            |   |-- README.md
            |   `-- frd-spec.md
            `-- usecase/
                `-- cot-usecases.md
```

## Notes

- Project folders under `src` and `test` are placeholders for now.
- No implementation project is created yet.
- Naming follows `TradingDashboard.<Layer>` convention.
- Blazor UI consumes data from `TradingDashboard.Presentation.Api` only.
- `TradingDashboard.Domain.Abstraction` is a dedicated project for shared base domain types.
