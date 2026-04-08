# User Management API Contract

## Base

- Base path: `/api/v1/login`

## Authentication Endpoints

### `POST /api/v1/login/signup`

- Creates a local account and returns JWT auth payload.

### `POST /api/v1/login/signin`

- Signs in with username or email plus password and returns JWT auth payload.

### `GET /api/v1/login/me`

- Returns the current authenticated profile.

### `GET /api/v1/login/google/start`

- Starts Google OAuth.

### `GET /api/v1/login/google/callback`

- Completes Google OAuth and redirects back to Blazor.

## Subscription Endpoints

### `GET /api/v1/login/subscriptions/plans`

- Auth: not required
- Returns the supported pricing plans for the Pricing page.

### `GET /api/v1/login/subscriptions/me`

- Auth: bearer token required
- Returns the current user subscription, if any.

### `POST /api/v1/login/subscriptions/purchase`

- Auth: bearer token required
- Accepts `planCode` and `billingCycle`.
- Activates or extends the user subscription.
