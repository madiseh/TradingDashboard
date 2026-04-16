# Trading Panel Shell

## Components

- Layout: `TradingPanelLayout`
- Page: `TradingPanelDashboard`
- Layout source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Pages/TradingPanel/_Layout/TradingPanelLayout.razor`
- Dashboard source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Pages/TradingPanel/_Dashboard/TradingPanelDashboard.razor`
- Resource markers:
  - `TradingPanelLayoutResource`
  - `TradingPanelDashboardResource`

## Purpose

Provide a separate trading workspace template with left navigation and a main content area for authenticated users from `Guest` upward.

## Notes

- Route access is guarded in the layout.
- This shell is the post-login landing area for non-admin users.
