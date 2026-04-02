# MAUI Blazor Hybrid Presentation Host

## Goal

Add a new Presentation host for `.NET MAUI Blazor Hybrid` alongside the existing Blazor web host.

## Why

- The solution needs a shared UI strategy for `Windows`, `Android`, and `iOS`.
- Existing investment is in Razor components and Blazor-oriented UI composition.
- The new host must preserve Clean Architecture boundaries and avoid pushing business logic into the client host.

## Scope

- Add a new project under `src/Presentation/`.
- Target `Windows`, `Android`, and `iOS`.
- Host Razor UI through `BlazorWebView`.
- Keep API communication through existing REST endpoints.
- Keep business logic in Application/Feature layers and not inside the MAUI project.

## Architectural Direction

1. The MAUI project is a Presentation host only.
2. The MAUI project may reference:
   - `TradingDashboard.Presentation.Common`
   - Existing presentation-oriented contracts/resources as needed
3. The MAUI project must not contain domain or application business rules.
4. Service registration for MAUI-specific presentation dependencies must be centralized in a dedicated `IocRegistration` entry point and called from `MauiProgram.cs`.
5. Cross-platform configuration must be read from app configuration files instead of hard-coded endpoint values when possible.

## Initial Implementation Plan

1. Create `TradingDashboard.Presentation.MauiHybrid` under `src/Presentation/`.
2. Enable `net9.0-android`, `net9.0-ios`, and `net9.0-windows10.0.19041.0`.
3. Add a Blazor Hybrid shell with a reusable root component and routing entry.
4. Reuse presentation contracts/resources where compatibility is already available.
5. Keep the first iteration focused on a stable host shell and API connectivity.
6. Document platform constraints explicitly, especially `iOS` build/publish requirements on Windows environments.

## Documentation Rule For This Host

- `README.md` must mention the new MAUI host and supported outputs.
- `project-structure.md` must mention the new Presentation project.
- Feature documentation must capture the functional scope of the host.
