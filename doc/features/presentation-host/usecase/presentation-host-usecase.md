# Presentation Host Use Case

## Launch Flow

1. User launches the MAUI application on `Windows`, `Android`, or `iOS`.
2. The MAUI host boots platform services and `BlazorWebView`.
3. The root Razor component loads inside the native shell.
4. The user navigates through the same Presentation routes exposed by the hybrid UI.

## Data Access Flow

1. Razor UI requests data through named `HttpClient` instances.
2. Calls are sent to the existing backend APIs.
3. Results are rendered inside the Blazor Hybrid shell.

## Authentication Flow

1. User signs in through the Presentation login experience.
2. JWT token is stored in Presentation session state.
3. Authenticated requests attach the bearer token before calling the API.
