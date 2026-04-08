# User Management Service Contract

## Responsibilities

- Manage local and Google-based authentication.
- Persist user identity state in the feature-owned database.
- Own subscription plan catalog for the Pricing page.
- Own the subscription activation and extension process.

## Application Services

### Authentication

- Sign up local users
- Sign in existing users
- Issue JWT tokens
- Resolve Google OAuth callback into local users

### Subscription

- Publish supported plan definitions
- Calculate yearly discounts
- Create or extend the authenticated user's subscription
- Return current subscription state to Blazor
