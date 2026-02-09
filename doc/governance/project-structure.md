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
|   |   |-- TradingDashboard.Infrastructure/
|   |   `-- Persistence/
|   |       |-- TradingDashboard.Infrastructure.Persistence.Common/
|   |       `-- TradingDashboard.Infrastructure.Persistence.Implementation/
|   `-- Presentation/
|       |-- TradingDashboard.Presentation.Api/
|       `-- TradingDashboard.Presentation.Blazor/
|-- test/
|   |-- TradingDashboard.UnitTests/
|   `-- TradingDashboard.IntegrationTests/
`-- doc/
    |-- README.md
    |-- governance/
    |   |-- architecture-principles.md
    |   `-- project-structure.md
    `-- features/
        |-- _template.md
        `-- cot/
            |-- contract/
            |   `-- api-contract.md
            |-- domain/
            |   `-- domain-contract.md
            |-- frd/
            |   `-- frd-spec.md
            `-- usecase/
                `-- cot-usecases.md
```

## Persistence Implementation Internal Structure

```text
src/Infrastructure/Persistence/TradingDashboard.Infrastructure.Persistence.Implementation/
|-- Configuration/
|   |-- BaseAuditableEntityConfiguration.cs
|   `-- BaseEntityConfiguration.cs
|-- Migrations/
|   |-- BaseDataMigration.cs
|   |-- BaseMigration.cs
|   `-- BaseSchemaMigration.cs
|-- Repositories/
|   `-- GenericRepository.cs
`-- GlobalUsings.cs
```

## Notes

- Project folders under `src` and `test` are placeholders for now.
- No implementation project is created yet.
- Naming follows `TradingDashboard.<Layer>` convention.
- Blazor UI consumes data from `TradingDashboard.Presentation.Api` only.
- `TradingDashboard.Domain.Abstraction` is a dedicated project for shared base domain types.
- Persistence is split into `Infrastructure.Persistence.Common` and `Infrastructure.Persistence.Implementation`.
