# Portal Access FRD

## Goal

Deliver role-based access entry for a separate Admin panel and a separate Trading panel.

## Functional Requirements

1. Public top menu must replace `Contact Us` with `Login` and `Sign Up`.
2. System must define three database-backed roles:
   - `SuperAdmin`
   - `Analyzer`
   - `Guest`
3. Seeded admin user must be `SuperAdmin`.
4. New sign-ups must default to `Guest`.
5. Admin panel must be a separate environment with:
   - Left panel
   - Main component area
6. Trading panel must also have its own separate template with:
   - Left panel
   - Main component area
7. Only `SuperAdmin` can access Admin panel.
8. `Guest` and above can access Trading panel.

## Acceptance Criteria

1. Successful `SuperAdmin` login redirects to Admin panel.
2. Successful `Guest` login redirects to Trading panel.
3. Admin seed record is stored as `SuperAdmin`.
4. New signup records are stored as `Guest`.
5. Guest users cannot remain inside Admin routes.
