# Login Use Case

## Local Sign-In

1. User enters username/email and password in Blazor Login page.
2. UI calls `POST /api/v1/login/signin`.
3. API validates credentials.
4. If local account email is not verified, API returns `EMAIL_NOT_VERIFIED`.
5. If sign-in succeeds, API returns JWT.
6. UI stores token in `AuthSession` and loads `/me`.

## Local Sign-Up

1. User enters username/email/password.
2. UI calls `POST /api/v1/login/signup`.
3. API creates user and returns JWT.
4. UI stores token and loads `/me`.
5. Created user attributes:
   - `SignupAppPlatform = Local`
   - `IsEmailVerified = false`
6. System sends an activation email notification after local account creation.

## Forgot Password

1. User enters username/email in Login page.
2. User clicks `Forgot Password`.
3. UI calls `POST /api/v1/login/forgot-password`.
4. If a verified active local account matches the identity, system sends password reset email notification.
5. UI shows success snackbar using API response message.

## Resend Activation Email

1. User attempts local sign-in with an unverified local account.
2. API returns `EMAIL_NOT_VERIFIED`.
3. UI reveals `Send Activation Email`.
4. User clicks `Send Activation Email`.
5. UI calls `POST /api/v1/login/resend-activation-email`.
6. If the matched local account is still unverified, system sends activation email notification.
7. UI shows success snackbar using API response message.

## Google Sign-In (Step by Step)

1. User clicks `Sign in with Google` in `/login` page.
2. Blazor navigates browser to `GET /api/v1/login/google/start?returnUrl={ui_login_absolute_url}`.
3. API triggers `Challenge` with Google scheme.
4. User authenticates/consents on Google page.
5. Google redirects back to API auth callback pipeline.
6. API endpoint `GET /api/v1/login/google/callback` reads external principal.
7. API resolves user by email:
   - if found: update login timestamp.
   - if not found: create local user with `SignupAppPlatform = Google` and `IsEmailVerified = true`, then mark login.
8. API generates JWT via `IJwtTokenService`.
9. API redirects browser to `returnUrl` with token-related query parameters.
10. Blazor login page reads query parameters, stores JWT in `AuthSession`, and fetches `/me`.
