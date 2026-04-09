# Feature: Pricing

## Objective

Add a dedicated public Pricing page to TerminalPrime and expose the premium subscription offer through the main site navigation.

## In Scope

- Top menu `Pricing` link
- Dedicated Blazor Pricing page
- Localized resource files for the page
- Monthly and yearly plan presentation
- Annual discount messaging
- Purchase CTA wired to the subscription process

## Out of Scope

- Third-party payment gateway integration
- Multi-plan billing catalog in phase 1

## Functional Requirements

1. The public top menu must contain a `Pricing` item.
2. Clicking the menu item must navigate to a dedicated `/pricing` page.
3. The page must use a dedicated resource marker with `en`, `fa`, `tr`, and `de` resource files.
4. The page must display TerminalPrime Premium in a layout tailored to the current website.
5. The page must allow the user to choose monthly or yearly billing.
6. The yearly option must clearly show the applied discount.
7. The purchase flow must work for authenticated users and guide unauthenticated users to login.

## Acceptance Criteria

1. A visitor can reach the Pricing page from the main menu.
2. The page renders localized content from its own resource files.
3. The page shows monthly and yearly purchase states for the premium plan.
4. An authenticated user can activate a subscription from the Pricing page.
