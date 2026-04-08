# Admin Users Domain

## AppUser additions

- `Role`
  - `Guest`
  - `Analyzer`
  - `SuperAdmin`

- `Status`
  - `Active`
  - `Disabled`

## Rules

1. New local and Google sign-ups are created as `Guest`.
2. New users start in `Active` status.
3. Seeded admin account must always be `SuperAdmin`.
4. Disabled users cannot sign in.
5. Password changes replace the stored hash and salt.
