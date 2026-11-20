# Architecture Principles

## Target Style

- Start as a modular monolith.
- Follow Clean Architecture.
- Design module boundaries to enable later microservice migration.

## Layers

1. Domain
- Entities, value objects, domain services, domain events.
- No dependency on frameworks.

2. Application
- Use cases, commands/queries, DTOs, validation, interfaces.
- Depends only on Domain.

3. Infrastructure
- Data access, external APIs, messaging, caching, file storage.
- Implements interfaces defined in Application.

4. Presentation
- API or UI endpoints.
- Depends on Application contracts only.
- The API host composition root may reference Infrastructure only to configure dependency injection and startup migrations; endpoint handlers must not reference Infrastructure.
- Blazor UI must use MudBlazor as the component library.
- Shared presentation contracts (e.g., route constants) must live in `TradingDashboard.Presentation.Common`.
- Controllers/endpoints must delegate use-case execution to Application via MediatR; no business logic in controllers.

## Dependency Rule

- Dependencies always point inward.
- Domain never depends on outer layers.
- Application never depends directly on Infrastructure.
- IoC/DI registrations must be centralized in dedicated `*IocRegistration` files and invoked from `Program.cs`.
- New service registrations must not be scattered inside `Program.cs`; they must be added to the relevant IoC registration file.
- Application persistence/service dependencies must be abstractions (`Application/Abstractions/*`) and implemented in Infrastructure.
- `DbContext` or EF Core query logic must not be used directly inside Application handlers.

## CQRS and MediatR Standard

- Every use case in Application must be implemented as MediatR request/handler.
- Request types must be `record` and live under `Application/Features/<FeatureName>/<Commands|Queries>/<UseCaseName>/`.
- Handler names must end with `Handler` and implement `IRequestHandler<TRequest, OperationalResult<TResponse>>`.
- Handler methods must be async, accept `CancellationToken`, and avoid static/global state.
- Response DTOs must be `record` types scoped to the same use-case folder.

Required folder shape:

```text
Application/
  Features/
    <FeatureName>/
      Commands|Queries/
        <UseCaseName>/
          <UseCaseName>Command|Query.cs
          <UseCaseName>Handler.cs
          <UseCaseName>Response.cs
```

## Operational Result Contract

- Application use cases must return `OperationalResult<TResponse>`.
- `OperationalResult<T>` must expose:
  - `IsSuccess`
  - `Message`
  - `ErrorCode`
  - `Errors`
  - `Data`
- Use standard error codes only:
  - `VALIDATION`
  - `DUPLICATE`
  - `NOT_FOUND`
  - `UNEXPECTED`
- Validation failures and business rule failures must be explicit via `OperationalResult.Fail(...)`.
- Unexpected exceptions in handlers must be caught and returned as `UNEXPECTED`.

## Validation and Business Rules

- Each command/query handler must include:
  - Basic input validation (required fields, minimum length, format if relevant).
  - At least one business rule validation (for example duplicate checks or not-found checks).
- Business rules must run in Application using repository abstractions, not in controllers.

## API Controller Rule

- Controllers should only:
  - Accept request input.
  - Call `_mediator.Send(request, cancellationToken)`.
  - Map `OperationalResult` to HTTP response.
- For failed results, API responses must include `{ Message, ErrorCode, Errors }`.
- Success responses should return `result.Data` with appropriate status code (`200` or `201` based on scenario).

## Microservice Readiness

- Keep feature modules cohesive.
- COT, CurrencyStrength, Login, and UserManagement share one project per Domain, Application, Infrastructure, and API layer. Keep their source folders, namespaces, and endpoint groups separated by feature.
- Login authentication and admin-user use cases belong to the UserManagement domain and persistence boundary where they share user identity data.
- Avoid shared mutable state across modules.
- Define explicit contracts per module.
- Use integration boundaries that can be extracted later.

## Seed Data Rule

- Seed data must be defined in `Infrastructure.Persistence.Implementation`.
- Seed classes should be static (e.g., `*SeedData`).
- `Program.cs` may call seed classes but should not embed seed records directly.

## Simulated Data Rule

- JSON-based fake repositories must live in `Infrastructure.Persistence.SimulatedData`.
- EF Core repositories remain in persistence implementation projects and represent real database access.
- EF entity mappings must live in `Infrastructure.Persistence.Implementation/Configuration` as `IEntityTypeConfiguration<T>`.
- `DbContext` classes must apply these mappings instead of inlining entity configuration in `OnModelCreating`.

## Database Table Naming Rule

- Database table names must be defined as constants in a dedicated file.
- EF Core mappings must use those constants (for example in `ToTable(...)`) and must not use hard-coded table name strings.

## Migration Version Folder Rule

- Every EF Core migration must be created inside a dedicated version folder under `Migrations/` (for example `Migrations/v1.0.1`).
- A single migration must not be placed in the root `Migrations/` directory.
- Version folders must be incremental and represent release/version grouping.
- Schema migrations must inherit from `BaseSchemaMigration`.
- Data migrations must inherit from `BaseDataMigration`.

## Global Usings Standard

- Every project must include a `GlobalUsings.cs` file.
- `GlobalUsings.cs` must contain a `#region System` block.
- All shared `using System...` directives must be placed inside that region.

## NuGet Versioning Rule

- Package versions must be managed centrally in `Directory.Packages.props`.
- `PackageReference` entries in `.csproj` files must not include a `Version` attribute.
- New packages must be added with `PackageVersion` in `Directory.Packages.props` before use in projects.

## Interface Documentation Rule

- Every interface must include XML documentation summary (`/// <summary> ... </summary>`).
- Every interface method must include XML documentation summary.
- Interface responsibilities and method names must also be documented in related feature Markdown files.

## Razor Component Structure Rule

- Every Blazor component must use split files: `<Component>.razor` and `<Component>.razor.cs`.
- Every Blazor page/module component must have a dedicated sibling view-model file: `<Component>.ViewModels.cs` in the same folder.
- Every first-level folder under `TradingDashboard.Presentation.Blazor/Components/Pages/<SectionName>/` must contain:
  - `_Layout/<SectionName>Layout.razor`
  - `_Dashboard/<SectionName>Dashboard.razor`
- Section layouts must live in their owning page section folder, not under the shared `Components/Layout` folder.
- Section dashboard pages must use the `Dashboard` naming convention. Do not name section landing pages `Home` or keep them in a `Home` folder.
- UI markup must stay in `.razor`.
- Parameters, state, and methods must stay in `.razor.cs`.
- View model records/classes must stay in `<Component>.ViewModels.cs`.
- Inline `@code` blocks should not be used in `.razor` files.

Example:

```csharp
#region System
global using System;
global using System.Collections.Generic;
global using System.Linq;
#endregion
```
