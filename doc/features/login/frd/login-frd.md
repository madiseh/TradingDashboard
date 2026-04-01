# Login FRD

## Goal

Deliver a complete login experience in Blazor with local credentials and Google OAuth entry from the Login page.

## Scope

- Login UI in Blazor (`/login`)
- Login API v1 endpoints
- JWT session bootstrap in UI
- Google OAuth integration

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

## Acceptance Criteria

1. User can click `Sign in with Google` and complete sign-in end-to-end.
2. A new Google user is auto-provisioned in local `AppUser` if not already present.
3. Existing Google-linked user signs in without creating duplicates.
4. Authenticated session allows successful call to `GET /api/v1/login/me`.
5. Local `signin` and `signup` continue to work unchanged.
6. A local sign-up record is stored with `SignupAppPlatform = Local` and `IsEmailVerified = false`.
7. A Google sign-up record is stored with `SignupAppPlatform = Google` and `IsEmailVerified = true`.
