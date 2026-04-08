# Admin Users FRD

## Goal

Deliver the first working admin users page for listing and basic account management.

## Scope

- Admin-only user list page in Blazor
- Search on the user directory
- Status and role visibility
- Direct password change action from the main list
- Role/status support in Login domain and API

## Functional Requirements

1. System must store role and status on each `AppUser`.
2. System must seed the built-in admin account as `SuperAdmin`.
3. System must block disabled users from sign-in.
4. Admin users page must display:
   - username
   - email
   - role
   - status
   - signup platform
   - email verification
   - created time
   - last login
5. Admin users page must provide a search box backed by the API.
6. Admin users page must let SuperAdmin change a user password inline.

## Acceptance Criteria

1. `GET /api/v1/login/admin/users` returns a populated list for a SuperAdmin token.
2. Search narrows the list by matching users.
3. Clicking `Change Password` updates the selected account password.
4. A disabled account cannot authenticate through sign-in or Google callback.
