# COT Breadcrumb UI Component

## Component

- Name: `CotBreadcrumb`
- Source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Modules/Breadcrumb/CotBreadcrumb.razor`

## Purpose

Render the breadcrumb path for COT report pages.

## Input

- `CurrentPageTitle` (`string`): text for the current page node.

## Behavior

- Shows `Home / COT Reports / {CurrentPageTitle}`.
- Uses `RoutePaths.Ui` constants for route links.

## Notes

- Must follow split-file rule: `.razor` + `.razor.cs`.
