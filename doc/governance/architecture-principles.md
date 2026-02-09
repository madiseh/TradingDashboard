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
- Blazor UI must use MudBlazor as the component library.

## Dependency Rule

- Dependencies always point inward.
- Domain never depends on outer layers.
- Application never depends directly on Infrastructure.
- IoC/DI registrations must be centralized in dedicated `*IocRegistration` files and invoked from `Program.cs`.
- New service registrations must not be scattered inside `Program.cs`; they must be added to the relevant IoC registration file.

## Microservice Readiness

- Keep feature modules cohesive.
- Avoid shared mutable state across modules.
- Define explicit contracts per module.
- Use integration boundaries that can be extracted later.

## Global Usings Standard

- Every project must include a `GlobalUsings.cs` file.
- `GlobalUsings.cs` must contain a `#region System` block.
- All shared `using System...` directives must be placed inside that region.

Example:

```csharp
#region System
global using System;
global using System.Collections.Generic;
global using System.Linq;
#endregion
```
