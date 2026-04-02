# MAUI Hybrid Home Shell

## Purpose

Provide the first cross-platform shell page for the MAUI Blazor Hybrid host.

## Structure

- Root route: `/`
- Layout: `MainLayout`
- Content:
  - hero section
  - platform status cards
  - configured backend endpoint summary

## Notes

- This page is Presentation-only and does not contain business logic.
- Localization resources live in `TradingDashboard.Presentation.Common`.
- The page is intentionally simple so the host can be validated on `Windows`, `Android`, and `iOS` before migrating larger Blazor modules.
