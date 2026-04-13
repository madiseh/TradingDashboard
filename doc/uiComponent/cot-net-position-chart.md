# COT Position Chart UI Component

## Component

- Name: `CotPositionChart`
- Source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Modules/Chart/CotPositionChart.razor`

## Purpose

Render the selectable multi-tab position chart card for a COT instrument.

## Input

- `InstrumentSlug` (`string`): the current market route slug.
- `Model` (`CotReportModel`): decides which participant groups are available.

## Behavior

- Loads `available-dates` from the COT API and initializes the date range from the most recent 52 report weeks.
- Renders three tabs: `Net Position`, `Long`, and `Short`.
- Renders participant-group toggle cards above the chart and keeps all groups selected by default.
- Renders `from` and `to` date pickers inside a filter panel and restricts selection to available report dates only.
- Calls the `series` endpoint per selected group and reuses the same chart module to plot `netPosition`, `long`, or `short` values based on the active tab.
- Does not render any `prices` dataset.

## Notes

- Must follow split-file rule: `.razor` + `.razor.cs` + `.ViewModels.cs`.
- The component owns filter state, API calls, chart geometry, and tab-to-metric mapping so the page only composes modules.
