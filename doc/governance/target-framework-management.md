# Target Framework Management

## Rule

- The solution target framework is managed centrally in `Directory.Build.props`.
- Standard projects must use `$(TradingDashboardTargetFramework)` for `<TargetFramework>`.
- MAUI projects must use `$(TradingDashboardMauiTargetFrameworks)` and append `$(TradingDashboardMauiWindowsTargetFramework)` for Windows builds.
- Project files must not hard-code `net10.0` target framework values unless a platform-specific SDK requirement cannot be represented by the central properties.

## Current Target

- `TradingDashboardTargetFramework`: `net10.0`
- `TradingDashboardMauiTargetFrameworks`: `net10.0-android;net10.0-ios`
- `TradingDashboardMauiWindowsTargetFramework`: `net10.0-windows10.0.19041.0`
