# Admin Shell

## Components

- Layout: `AdminLayout`
- Page: `AdminDashboard`
- Resource markers:
  - `AdminLayoutResource`
  - `AdminDashboardResource`

## Purpose

Provide a fully separate Admin environment with a persistent left panel and a main dashboard surface reserved for `SuperAdmin`.

## Notes

- Route access is guarded in the layout.
- The shell is isolated from the public site header/footer.
