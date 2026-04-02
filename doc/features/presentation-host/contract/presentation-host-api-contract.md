# Presentation Host API Contract

## Purpose

Define how the MAUI Blazor Hybrid host communicates with backend APIs.

## Host-to-API Contract

- The host consumes the existing REST APIs exposed by feature presentation API projects.
- API base URLs are configured through application settings.
- The host creates named API clients through a Presentation-layer factory for backend communication.

## Current Named Clients

- `ApiCot`
- `ApiLogin`

## Authentication Contract

- Authentication tokens are stored in Presentation-layer session state only.
- Authenticated calls must apply the bearer token to the named `HttpClient`.
- The host must remain compatible with the existing login API contract.
