# COT Long/Short Chart UI Component

## Component

- Name: `CotLongShortChart`
- Source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Modules/Chart/CotLongShortChart.razor`

## Purpose

Render a reusable visual summary of long versus short positions for the current COT report.

## Input

- `Model` (`CotReportModel`): decides which report categories are plotted.
- `Report` (`CotLatestReportResponse?`): supplies categories and position values.

## Behavior

- `CotLegacy` model: renders `Non-Commercial`, `Commercial`, and `Non-Reportable`.
- `Cot` model: renders `Dealer`, `Asset Manager`, `Leveraged Funds`, `Other Reportables`, and `Non-Reportable`.
- Uses long and short position totals to calculate each stacked bar ratio.

## Notes

- Must follow split-file rule: `.razor` + `.razor.cs`.
- Category selection and ratio calculation stay inside the component so the page only composes modules.
