# Admin Users Use Case

## Primary use cases

1. SuperAdmin opens the admin users page.
2. System lists all users with role, status, signup platform, verification, created date, and last login.
3. SuperAdmin searches by username, email, role, or status.
4. SuperAdmin changes a user password directly from the list.

## Error handling

- If the user is not authenticated, the page must ask for sign-in.
- If the user is authenticated but not `SuperAdmin`, API returns `403`.
- If password change validation fails, the UI must show the API error message.
