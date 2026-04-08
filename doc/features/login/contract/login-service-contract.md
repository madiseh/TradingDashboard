# Login Service Contract

## Purpose

Provide authentication services for local credentials and Google OAuth.

## Services

### `IJwtTokenService`

- `GenerateToken(AuthUserResponse user)`
- Returns JWT payload with expiration and user profile fields.

### `IPasswordHashingService`

- `CreateHash(password)`
- `Verify(password, hash, salt)`

## External Authentication Contract

- Scheme name: `Google`
- Temporary sign-in cookie scheme: `External`
- API must support:
  - challenge start (`google/start`)
  - callback completion (`google/callback`)

## User Persistence Contract

- `AppUser.SignupAppPlatform`: enum values `Local` and `Google`.
- `AppUser.IsEmailVerified`:
  - default `false` for local sign-up.
  - set to `true` for Google sign-up.

## Configuration Contract

- `Authentication:Google:ClientId`
- `Authentication:Google:ClientSecret`
- `Jwt:*`
- `ConnectionStrings:UserManagementDatabase`
