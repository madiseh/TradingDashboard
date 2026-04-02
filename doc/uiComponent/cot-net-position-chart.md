# COT Net Position Chart UI Component

## Component

- Name: `CotNetPositionChart`
- Source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Modules/Chart/CotNetPositionChart.razor`

## Purpose

Render the selectable net-position chart card for a COT instrument.

## Input

- `InstrumentSlug` (`string`): the current market route slug.
- `Model` (`CotReportModel`): decides which participant groups are available.

## Behavior

- Loads `available-dates` from the COT API and initializes the date range from the most recent 52 report weeks.
- Renders participant-group filters inside an accordion section and keeps all groups selected by default.
- Renders `from` and `to` date pickers inside an accordion section and restricts selection to available report dates only.
- Calls the `series` endpoint per selected group and plots only `netPosition` values.
- Does not render any `prices` dataset.

## Notes

- Must follow split-file rule: `.razor` + `.razor.cs` + `.ViewModels.cs`.
- The component owns filter state, API calls, chart geometry, and line rendering so the page only composes modules.
