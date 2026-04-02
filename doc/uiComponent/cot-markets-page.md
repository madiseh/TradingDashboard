# COT Markets Page

## Component

- Name: `Markets`
- Source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Pages/Markets/Markets.razor`

## Purpose

Render a dedicated page for one COT market group selected from the home directory links.

## Route Contract

- Route: `/markets`
- Query:
  - `group`: market group name such as `Currencies` or `Cryptocurrencies`
  - `model`: report model value such as `cot` or `cot-legacy`

## Behavior

- Loads the selected group from `GET /api/v1/cot/instruments/by-group?group=...`.
- Shows only the requested group and its own instruments.
- Keeps the selected `model` and uses it when building report links for each instrument.
- Provides a return action back to the markets directory page.

## Notes

- Follows the split-file page rule: `.razor` + `.razor.cs` + `.ViewModels.cs`.
- Uses a dedicated localized resource set under `TradingDashboard.Presentation.Common`.
