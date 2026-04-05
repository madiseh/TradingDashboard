# Top Menu

## Components

- Module: `TopMenu`
- Resource marker: `TopMenuResource`

## Purpose

Provide a reusable top navigation module for the public-facing shell instead of keeping header markup inside `MainLayout`.

## Notes

- Localization resources live in `TradingDashboard.Presentation.Common`.
- The module owns all header, off-canvas, language-picker, and header search texts.
- `MainLayout` remains responsible for shell chrome such as footer and error banner.
