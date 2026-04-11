# Cookie Consent FRD

## Goal

Add the CookieYes consent mode banner script to the Blazor web host.

## Scope

- Load the CookieYes client script for client data ID `75a44bedb1c4246b0619bb80`.
- Include the script once from the shared Blazor HTML shell so it applies to every routed page.
- Place the consent script before analytics scripts in the document head.

## Functional Requirements

1. The Blazor web host must load `https://cdn-cookieyes.com/client_data/75a44bedb1c4246b0619bb80/script.js`.
2. The script tag must use the `cookieyes` id.
3. The script tag must be rendered inside the shared `<head>` in `Components/App.razor`.
4. The consent script must be placed before the Google Analytics tag loader.

## Non-Functional Requirements

1. The CookieYes script must not be duplicated per page component.
2. The integration must not introduce Domain or Application layer dependencies.

## Acceptance Criteria

1. The rendered `<head>` includes the CookieYes script.
2. The CookieYes script appears before the Google Analytics script.
3. Routed Blazor pages share the same CookieYes script inclusion.
