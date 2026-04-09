# Portal Access Domain

## User Role Model

`AppUser` now includes `Role`.

Defined roles:

- `SuperAdmin`
- `Analyzer`
- `Guest`

## Domain Rules

1. Seeded admin user must be `SuperAdmin`.
2. Local sign-up must default to `Guest`.
3. Auto-provisioned Google users must default to `Guest`.
4. Only `SuperAdmin` can enter the isolated Admin panel.
5. Any authenticated role from `Guest` upward can enter `TradingPanel`.
