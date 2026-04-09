# Feature: User Management

## Objective

Expand the current login feature into a broader User Management feature that owns authentication and subscription lifecycle management.

## In Scope

- Existing sign-up, sign-in, and Google OAuth flows
- Internal feature/project rename from `Login` to `UserManagement`
- Subscription table and persistence
- Monthly and yearly subscription activation
- Annual discount support
- API endpoints required by the Pricing page

## Out of Scope

- Payment gateway integration
- Coupons and promotional code engine
- Team or family subscriptions

## Functional Requirements

1. The feature must continue to support local sign-up and sign-in.
2. The feature must continue to support Google OAuth sign-in.
3. The feature must expose subscription plan metadata for the Pricing page.
4. The feature must persist a subscription table in the feature database.
5. The feature must activate monthly and yearly subscriptions.
6. The yearly subscription path must apply a discount.
7. If the user already has time remaining, a new purchase must extend the remaining period.

## Acceptance Criteria

1. The solution shows the feature as `UserManagement`.
2. Existing login flows continue to build and run.
3. A subscription record is created or updated when a user purchases a plan.
4. Monthly and yearly purchase flows return different billing details, with yearly showing a discount.
