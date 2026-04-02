# Presentation Host FRD

## Goal

Provide a MAUI Blazor Hybrid client for the Trading Dashboard solution.

## Scope

- New Presentation project for:
  - `Windows`
  - `Android`
  - `iOS`
- Blazor Hybrid shell and startup pipeline
- Configuration-driven API connectivity
- Documentation for platform support and structure

## Functional Requirements

1. The solution must contain a MAUI Blazor Hybrid project next to the existing Blazor web project.
2. The project must target `Windows`, `Android`, and `iOS`.
3. The project must boot a `BlazorWebView` host successfully.
4. The project must expose a root Razor route experience suitable for future component reuse.
5. The project must be added to the solution file and repository documentation.

## Non-Functional Requirements

1. The MAUI host must remain a Presentation-only layer.
2. Configuration must be externalized through app settings.
3. Service registration must remain centralized.
4. Initial implementation must avoid unnecessary duplication of domain/application logic.

## Acceptance Criteria

1. Developers can open the solution and see the new MAUI project.
2. The MAUI project declares `Windows`, `Android`, and `iOS` targets.
3. A Windows-target build can be validated on this machine.
4. Platform-specific constraints for `iOS` are documented.
