# Portal Access Use Case

## Public Navigation

1. Visitor lands on the public site.
2. Top menu shows `Login` and `Sign Up` instead of `Contact Us`.
3. `Sign Up` opens the login page in sign-up mode.

## Sign-Up Flow

1. User completes sign-up.
2. API creates the user with `Role = Guest`.
3. UI stores JWT and role.
4. UI redirects the user to `TradingPanel`.

## Sign-In Flow

1. User signs in successfully.
2. API returns JWT and role.
3. UI redirects by role:
   - `SuperAdmin` -> `Admin`
   - `Analyzer` -> `TradingPanel`
   - `Guest` -> `TradingPanel`

## Route Guard Flow

1. Unauthenticated user requests `Admin` or `TradingPanel`.
2. UI redirects user to `/login?returnUrl=...`.
3. After login:
   - If the role can access the requested protected route, UI returns there.
   - Otherwise UI redirects to the default role destination.
