# Cookie Consent Use Case

## Purpose

Describe how CookieYes is loaded for routed Blazor pages.

## Use Case: Load Cookie Consent On Page Shell

- Input: browser requests the Blazor web host.
- Flow:
  1. `Components/App.razor` renders the shared HTML document.
  2. The `<head>` loads the CookieYes consent script.
  3. The `<head>` loads analytics scripts after the consent script.
- Output: CookieYes consent mode is available for all routed Blazor pages served by the shared shell.

## Validation Rules

1. Do not add duplicate CookieYes scripts inside page components.
2. Keep the consent script before analytics scripts in the shared head.
