# Admin Shell

## Components

- Layout: `AdminLayout`
- Page: `AdminDashboard`
- Layout source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Pages/Admin/_Layout/AdminLayout.razor`
- Dashboard source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Pages/Admin/_Dashboard/AdminDashboard.razor`
- Resource markers:
  - `AdminLayoutResource`
  - `AdminDashboardResource`

## Purpose

Provide a fully separate Admin environment with a persistent left panel and a main dashboard surface reserved for `SuperAdmin`.

## Notes

- Route access is guarded in the layout.
- The shell is isolated from the public site header/footer.
