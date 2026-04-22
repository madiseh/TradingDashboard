# Login API Contract

## Base

- Base path: `/api/v1/login`

## Endpoints

### `POST /api/v1/login/signup`

- Description: Creates a new local account and returns JWT.
- Request body:
  - `username` (string)
  - `email` (string)
  - `password` (string)
- Response `200`:
  - `accessToken`
  - `expiresAtUtc`
  - `userId`
  - `username`
  - `email`

### `POST /api/v1/login/signin`

- Description: Signs in with username/email + password and returns JWT.
- Request body:
  - `identity` (string: username or email)
  - `password` (string)
- Response `200`: same shape as sign-up.
- Response `400`:
  - `errorCode = EMAIL_NOT_VERIFIED` when local account email has not been activated.

### `POST /api/v1/login/forgot-password`

- Description: Requests password reset for a local account.
- Request body:
  - `identity` (string: username or email)
- Response `200`:
  - `message`
- Response `400`:
  - `errorCode = EMAIL_NOT_VERIFIED` when local account email has not been activated yet.

### `POST /api/v1/login/resend-activation-email`

- Description: Requests a new activation email for a local account that is not verified.
- Request body:
  - `identity` (string: username or email)
- Response `200`:
  - `message`

### `GET /api/v1/login/me`

- Description: Returns current authenticated user based on JWT.
- Auth: `Bearer` token required.
- Response `200`:
  - `userId`
  - `username`
  - `email`

### `GET /api/v1/login/google/start?returnUrl={absolute_ui_url}`

- Description: Starts Google OAuth challenge.
- Query:
  - `returnUrl` (required, absolute URL of Blazor login page)
- Behavior:
  - Redirects browser to Google sign-in page.
  - On success/failure returns to `google/callback`.

### `GET /api/v1/login/google/callback?returnUrl={absolute_ui_url}`

- Description: Completes Google OAuth, creates/fetches local user, issues JWT, then redirects to UI.
- Query:
  - `returnUrl` (required)
- Success redirect query parameters appended to `returnUrl`:
  - `accessToken`
  - `expiresAtUtc`
  - `userId`
  - `username`
  - `email`
- Failure redirect query parameters:
  - `authError`
