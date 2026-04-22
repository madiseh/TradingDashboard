# Login Domain

## Core Entity

### `AppUser`

- `Id` (Guid)
- `Username` (unique)
- `Email` (unique)
- `PasswordHash`
- `PasswordSalt`
- `SignupAppPlatform` (enum: `Local`, `Google`)
- `IsEmailVerified` (bool)
- `LastLoginAtUtc`

## Domain Rules

1. Username and email must be unique.
2. Local sign-up requires password hash and salt.
3. Google sign-in links to local `AppUser` by email.
4. If Google email does not exist, a new local user is provisioned automatically with `SignupAppPlatform = Google`.
5. Local sign-up creates user with `SignupAppPlatform = Local` and `IsEmailVerified = false`.
6. Google sign-in creates user with `IsEmailVerified = true`.
7. Every successful sign-in updates `LastLoginAtUtc`.
8. Local credential sign-in is allowed only when `IsEmailVerified = true`.
9. Password reset email is sent only for verified local accounts.
10. Activation email resend is relevant only for local accounts with `IsEmailVerified = false`.

## Identity Mapping

- JWT claims generated from local `AppUser` identity.
- Google identity is only used for external verification; local user remains source of truth.
