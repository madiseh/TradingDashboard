# COT Report Page UI Component

## Component

- Name: `CotReport`
- Source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Pages/CotReport.razor`

## Purpose

Render the report page shell and compose report modules.

## Route Contract

- Route: `/cot/{instrumentSlug}`
- Query: `model` with values like `Cot` and `CotLegacy`

## Behavior

- Renders breadcrumb module.
- Renders report header and summary text.
- Renders `CotReportTable` based on selected model.
- Renders `CotNetPositionChart` for historical net-position analysis.

## Notes

- Must follow split-file rule: `.razor` + `.razor.cs`.
