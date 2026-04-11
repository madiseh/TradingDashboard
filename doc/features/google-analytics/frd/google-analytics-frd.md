# Google Analytics FRD

## Goal

Add Google Analytics measurement to all public Blazor web pages.

## Scope

- Load the Google tag for measurement ID `G-PDMZG45GBL`.
- Keep the local initialization code in a dedicated static JavaScript file.
- Include the tag once from the shared Blazor HTML shell so it applies to every routed page.

## Functional Requirements

1. The Blazor web host must load `https://www.googletagmanager.com/gtag/js?id=G-PDMZG45GBL`.
2. The Blazor web host must initialize `window.dataLayer`.
3. The Blazor web host must expose `window.gtag`.
4. The Blazor web host must call `gtag("js", new Date())`.
5. The Blazor web host must call `gtag("config", "G-PDMZG45GBL")`.

## Non-Functional Requirements

1. The Google tag must be included from `Components/App.razor`, the shared Blazor shell.
2. The local initialization script must live under `wwwroot/js`.
3. The Google tag must not be duplicated per page component.

## Acceptance Criteria

1. The rendered `<head>` includes the Google tag loader.
2. The rendered `<head>` includes the local Google Analytics initialization script.
3. Routed Blazor pages share the same Google Analytics tag inclusion.
