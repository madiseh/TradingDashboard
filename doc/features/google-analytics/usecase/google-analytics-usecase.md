# Google Analytics Use Case

## Purpose

Describe how Google Analytics is loaded for routed Blazor pages.

## Use Case: Load Analytics On Page Shell

- Input: browser requests the Blazor web host.
- Flow:
  1. `Components/App.razor` renders the shared HTML document.
  2. The `<head>` loads the Google tag script for `G-PDMZG45GBL`.
  3. The `<head>` loads the local Google Analytics initialization script.
  4. The local script initializes `window.dataLayer`, exposes `window.gtag`, and configures the measurement ID.
- Output: Google Analytics is available for all routed Blazor pages served by the shared shell.

## Validation Rules

1. Do not add duplicate Google tag snippets inside page components.
2. Keep the local initialization script in a dedicated static file.
