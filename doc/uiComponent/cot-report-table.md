# COT Report Table UI Component

## Component

- Name: `CotReportTable`
- Source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Modules/Table/CotReportTable.razor`

## Purpose

Render COT tabular data in two layouts.

## Input

- `Model` (`string?`): decides which layout is displayed.

## Behavior

- `CotLegacy` model: renders legacy table layout.
- Other models: renders standard COT table layout.

## Notes

- Must follow split-file rule: `.razor` + `.razor.cs`.
