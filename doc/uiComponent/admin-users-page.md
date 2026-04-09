# Admin Users Page

## Purpose

Provide the first admin-focused user management page in Blazor.

## Structure

- Route: `/admin/users`
- Layout:
  - left admin sidebar
  - hero summary panel
  - statistics cards
  - searchable user table
  - inline password change panel

## Notes

- The page intentionally keeps all actions on one surface so the first admin workflow is fast.
- Localization resources live under `TradingDashboard.Presentation.Common`.
- Search is server-backed through the Login API admin endpoints.
