# Presentation Host Service Contract

## Purpose

Describe Presentation-only services required by the MAUI Blazor Hybrid host.

## Services

### `IocRegistration`

- Central registration entry point for MAUI presentation services.
- Registers Presentation-only client creation services.
- Registers presentation session services.

### `ApiHttpClientFactory`

- Creates API clients by logical name.
- Resolves platform-aware base URLs from configuration.
- Uses Android emulator overrides when configured.

### `AuthSession`

- Stores access token and expiration in Presentation scope.
- Applies bearer token to outgoing HTTP clients when required.

## Configuration Contract

- `ApiCot:BaseUrl`
- `ApiLogin:BaseUrl`
