# Breadcrumb UI Component

## Purpose

Provide a reusable breadcrumb module for COT report pages and other navigation-heavy pages.

## Current Component

- Component: `CotBreadcrumb`
- Source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Modules/Breadcrumb/CotBreadcrumb.razor`

## Responsibilities

- Show navigation path for current page context.
- Keep route links centralized by using `RoutePaths` from `TradingDashboard.Presentation.Common`.
- Accept current page title as input and render a readable trail.

## Input Contract

- `CurrentPageTitle` (`string`): final breadcrumb node text for the current page.

## Output/Behavior

- Renders links for base levels (for example: Home, COT Reports).
- Renders a non-link final node for the current page title.

## Rules

- No hardcoded route strings in the component.
- Route constants must come from `RoutePaths`.
- Visual style should stay aligned with MudBlazor typography and links.

## Reuse Guideline

Create additional breadcrumb components under the same module path when a feature requires different navigation hierarchy.
