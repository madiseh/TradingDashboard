# COT Report Page UI Component

## Component

- Name: `CotReport`
- Source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Pages/Workspace/Cot/Report/CotReport.razor`

## Purpose

Render the report page shell and compose report modules.

## Route Contract

- Route: `/cot/report/{instrumentSlug}`
- Query: `model` with values like `cot` and `cot-legacy`

## Behavior

- Renders breadcrumb module.
- Renders report header and summary text.
- Renders an as-of date picker constrained to collected report dates.
- Renders previous and next arrow buttons to move between available report snapshots.
- Disables the left and right navigation buttons when there is no earlier or later collected snapshot.
- Renders `CotReportTable` based on selected model.
- Renders `CotLongShortChart` and `CotPositionChart` for historical positioning analysis.

## Notes

- Must follow split-file rule: `.razor` + `.razor.cs`.
