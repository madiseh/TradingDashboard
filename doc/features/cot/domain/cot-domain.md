# COT Domain Contract

## Purpose

Define domain building blocks for COT module and shared abstraction layer.

## Projects

- `TradingDashboard.Domain.Abstraction`
- `TradingDashboard.Domain`
- `TradingDashboard.Infrastructure.Persistence.Common`
- `TradingDashboard.Infrastructure.Persistence.Implementation`

## `TradingDashboard.Domain.Abstraction`

This is a separate project used by all domain modules.

### Required Base Types

- `BaseEntity<TId>`
- Holds `Id` and domain identity equality.
- Exposes protected constructor.

- `BaseAggregateRoot<TId>`
- Inherits from `BaseEntity<TId>`.
- Keeps domain events collection and clear/add helpers.

- `BaseValueObject`
- Implements structural equality via atomic values.
- Immutable-by-design.

- `IDomainEvent`
- Marker interface for domain events.

- `IHasDomainEvents`
- Exposes read-only domain events and clear method.

- `BusinessRuleValidationException`
- Thrown when domain invariants are violated.

### Optional Cross-Cutting Contracts

- `IAuditableEntity` (`CreatedAtUtc`, `UpdatedAtUtc`)
- `ISoftDelete` (`IsDeleted`, `DeletedAtUtc`)

## `TradingDashboard.Domain` (COT Module)

### Enums

- `ReportModel`: `Cot`, `CotLegacy`
- `ParticipantGroup`:
- Legacy: `NonCommercial`, `Commercial`, `NonReportable`
- Cot: `DealerIntermediary`, `AssetManagerInstitutional`, `LeveragedFunds`, `OtherReportables`, `NonreportablePositions`

### Entities / Aggregates

- `CotReport : BaseAggregateRoot<long>`
- `Instrument : BaseEntity<Guid>`
- `CotPosition : BaseEntity<long>`

### Value Objects

- `ReportDate`
- `PositionMetrics`
- `ChangeMetrics`
- `ModelSpecificMetrics`

### Domain Services

- `CotNormalizationService`
- `CotAggregationService`

### Repository Interfaces

- `ICotReportRepository`
- `ICotPositionRepository`
- `IInstrumentRepository`

## Domain Rules

1. One report per (`Instrument`, `ReportDate`, `ReportModel`).
2. Group must be valid for selected `ReportModel`.
3. `NetPosition = Long - Short`.
4. `Long`, `Short`, `OpenInterest` cannot be negative.
5. `ReportDate` must exist in official COT weekly calendar.
6. Arbitrary non-report calendar dates are invalid.

## Suggested Namespace Layout

- `TradingDashboard.Domain.Abstraction.Common`
- `TradingDashboard.Domain.Abstraction.Events`
- `TradingDashboard.Domain.Cot.Aggregates`
- `TradingDashboard.Domain.Cot.Entities`
- `TradingDashboard.Domain.Cot.ValueObjects`
- `TradingDashboard.Domain.Cot.Enums`
- `TradingDashboard.Domain.Cot.Repositories`
- `TradingDashboard.Domain.Cot.Services`

## Persistence Projects (Expected)

### `TradingDashboard.Infrastructure.Persistence.Common`

- Shared persistence contracts and cross-cutting persistence helpers.
- Base abstractions used by implementation project.

### `TradingDashboard.Infrastructure.Persistence.Implementation`

- `Configuration/`
- `BaseAuditableEntityConfiguration.cs`
- `BaseEntityConfiguration.cs`
- `Migrations/`
- `BaseDataMigration.cs`
- `BaseMigration.cs`
- `BaseSchemaMigration.cs`
- `Repositories/`
- `GenericRepository.cs`
- `GlobalUsings.cs`
