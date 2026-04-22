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

### `IAuthNotificationService`

- `SendActivationEmailAsync(AppUser user, CancellationToken cancellationToken)`
- `SendPasswordResetEmailAsync(AppUser user, CancellationToken cancellationToken)`
- Implementation must be replaceable so the current notification transport can be swapped with a real email provider.

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
  - blocks local sign-in until verified.

## Configuration Contract

- `Authentication:Google:ClientId`
- `Authentication:Google:ClientSecret`
- `Jwt:*`
- `ConnectionStrings:UserManagementDatabase`
