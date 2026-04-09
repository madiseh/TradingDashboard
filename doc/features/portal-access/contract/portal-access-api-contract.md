# Portal Access API Contract

## Login Response Extensions

- `POST /api/v1/login/signin`
- `POST /api/v1/login/signup`
- Google callback redirect payload
- `GET /api/v1/login/me`

All auth responses now include:

- `role`

## Role Values

- `SuperAdmin`
- `Analyzer`
- `Guest`

## Notes

- JWT tokens must carry the user role claim.
- `/me` must return the same role that was issued during authentication.
