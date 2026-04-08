# User Management Domain

## Core Entities

### `AppUser`

- `Id`
- `Username`
- `Email`
- `PasswordHash`
- `PasswordSalt`
- `SignupAppPlatform`
- `IsEmailVerified`
- `LastLoginAtUtc`

### `Subscription`

- `Id`
- `UserId`
- `PlanCode`
- `PlanName`
- `BillingCycle`
- `Status`
- `BasePriceUsd`
- `DiscountPercentage`
- `FinalPriceUsd`
- `StartsAtUtc`
- `ExpiresAtUtc`
- `PurchasedAtUtc`

## Rules

1. A user may sign up locally or through Google.
2. Subscription purchase supports `Monthly` and `Yearly` cycles.
3. Yearly purchase must include a discount.
4. Purchasing while a subscription is still active must extend the active period instead of losing remaining time.
5. The feature remains the source of truth for both authentication and subscription state.
