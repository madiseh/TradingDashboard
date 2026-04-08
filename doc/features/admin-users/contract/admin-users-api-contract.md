# Admin Users API Contract

## Base

- Base path: `/api/v1/login/admin/users`
- Auth: `Bearer` token required
- Role: `SuperAdmin`

## Endpoints

### `GET /api/v1/login/admin/users?search={value}`

- Description: Returns the admin user list and summary counters.
- Query:
  - `search` (optional)
- Response `200`:
  - `totalUsers`
  - `activeUsers`
  - `disabledUsers`
  - `superAdminUsers`
  - `users[]`
    - `userId`
    - `username`
    - `email`
    - `role`
    - `status`
    - `signupPlatform`
    - `isEmailVerified`
    - `createdAtUtc`
    - `updatedAtUtc`
    - `lastLoginAtUtc`

### `POST /api/v1/login/admin/users/{userId}/change-password`

- Description: Changes the password of the selected user.
- Request body:
  - `newPassword`
- Response `200`:
  - `userId`
  - `message`
