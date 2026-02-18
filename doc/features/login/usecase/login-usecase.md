# Login Use Case

## Local Sign-In

1. User enters username/email and password in Blazor Login page.
2. UI calls `POST /api/v1/login/signin`.
3. API validates credentials and returns JWT.
4. UI stores token in `AuthSession` and loads `/me`.

## Local Sign-Up

1. User enters username/email/password.
2. UI calls `POST /api/v1/login/signup`.
3. API creates user and returns JWT.
4. UI stores token and loads `/me`.

## Google Sign-In (Step by Step)

1. User clicks `Sign in with Google` in `/login` page.
2. Blazor navigates browser to `GET /api/v1/login/google/start?returnUrl={ui_login_absolute_url}`.
3. API triggers `Challenge` with Google scheme.
4. User authenticates/consents on Google page.
5. Google redirects back to API auth callback pipeline.
6. API endpoint `GET /api/v1/login/google/callback` reads external principal.
7. API resolves user by email:
   - if found: update login timestamp.
   - if not found: create local user, then mark login.
8. API generates JWT via `IJwtTokenService`.
9. API redirects browser to `returnUrl` with token-related query parameters.
10. Blazor login page reads query parameters, stores JWT in `AuthSession`, and fetches `/me`.
