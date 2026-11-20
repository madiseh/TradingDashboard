# Login FRD

## Goal

Deliver a complete login experience in Blazor with local credentials and Google OAuth entry from the Login page, including recovery actions for password reset and email activation.

## Scope

- Login UI in Blazor (`/login`)
- Login API v1 endpoints
- JWT session bootstrap in UI
- Google OAuth integration
- Forgot password request from Login page
- Resend activation email request for local users whose email is not verified

## Functional Requirements

1. Login page must include a `Sign in with Google` button in `Sign In` tab.
2. Clicking Google button must start OAuth flow through Login API.
3. After successful Google authentication, user must be authenticated in the app using JWT.
4. Existing local credentials flow (`signin`, `signup`) must remain functional.
5. `/me` endpoint must return the authenticated profile for local and Google flows.
6. System must persist signup source platform for each user (`Local`, `Google`).
7. System must persist email verification state:
   - `false` for local sign-up by default.
   - `true` for Google sign-up.
8. Local sign-in must be blocked when `IsEmailVerified = false`.
9. Login page must expose a `Forgot Password` action for local accounts.
10. When local sign-in fails because the email is not verified, Login page must expose a `Send Activation Email` action.
11. Forgot-password and resend-activation requests must return success responses that the UI can surface directly.

## Configuration Requirements

1. Google OAuth credentials are read from:
   - `Authentication:Google:ClientId`
   - `Authentication:Google:ClientSecret`
2. JWT options remain under `Jwt` section.
3. Local development callback in Google Console must include API callback endpoint (`/signin-google` path for ASP.NET Core Google handler).

## UX Requirements

1. Login page should show success snackbar after Google sign-in.
2. If Google sign-in fails, login page should show error alert/message.
3. Existing username/password fields remain available.
4. If sign-in fails because the email is not verified, page should clearly explain the reason and offer `Send Activation Email`.
5. Forgot-password flow should accept the same username/email identity field used by local sign-in.

## Acceptance Criteria

1. User can click `Sign in with Google` and complete sign-in end-to-end.
2. A new Google user is auto-provisioned in local `AppUser` if not already present.
3. Existing Google-linked user signs in without creating duplicates.
4. Authenticated session allows successful call to `GET /api/v1/login/me`.
5. Local `signin` and `signup` continue to work unchanged.
6. A local sign-up record is stored with `SignupAppPlatform = Local` and `IsEmailVerified = false`.
7. A Google sign-up record is stored with `SignupAppPlatform = Google` and `IsEmailVerified = true`.
8. A local user with `IsEmailVerified = false` cannot complete `signin`.
9. Login page allows submitting `POST /api/v1/login/forgot-password`.
10. Login page allows submitting `POST /api/v1/login/resend-activation-email` after `EMAIL_NOT_VERIFIED` response.
