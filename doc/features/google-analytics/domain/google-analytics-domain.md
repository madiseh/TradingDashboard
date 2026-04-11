# Google Analytics Domain

## Purpose

Document that Google Analytics is presentation instrumentation and has no domain model.

## Concepts

- `MeasurementId`: Google Analytics measurement identifier, currently `G-PDMZG45GBL`.
- `GoogleTagLoader`: external Google tag script loaded from Google Tag Manager.
- `GoogleAnalyticsInitializer`: local static JavaScript file that configures `window.gtag`.

## Notes

- This feature does not add entities, value objects, aggregates, or domain events.
- This feature must not introduce business logic into Domain or Application layers.
