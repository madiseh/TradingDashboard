# Login Domain

## Core Entity

### `AppUser`

- `Id` (Guid)
- `Username` (unique)
- `Email` (unique)
- `PasswordHash`
- `PasswordSalt`
- `LastLoginAtUtc`

## Domain Rules

1. Username and email must be unique.
2. Local sign-up requires password hash and salt.
3. Google sign-in links to local `AppUser` by email.
4. If Google email does not exist, a new local user is provisioned automatically.
5. Every successful sign-in updates `LastLoginAtUtc`.

## Identity Mapping

- JWT claims generated from local `AppUser` identity.
- Google identity is only used for external verification; local user remains source of truth.
