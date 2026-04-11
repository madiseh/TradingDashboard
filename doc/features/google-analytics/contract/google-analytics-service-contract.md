# Google Analytics Service Contract

## Purpose

Define the presentation-level integration surface for Google Analytics.

## Browser Globals

- `window.dataLayer`: Google tag event queue.
- `window.gtag`: helper function that pushes arguments into `window.dataLayer`.

## Ownership

- The Blazor web host owns script inclusion.
- The local initialization file owns browser global setup and configuration.
