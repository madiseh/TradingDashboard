# Pricing Use Cases

## UC1: Open Pricing Page

1. Visitor clicks `Pricing` in the top menu.
2. Blazor opens `/pricing`.
3. The page loads plan metadata and renders the premium offer.

## UC2: Compare Billing Cycles

1. Visitor switches between monthly and yearly purchase modes.
2. The page updates displayed price, savings, and CTA copy.

## UC3: Buy Subscription

1. Authenticated user clicks the purchase CTA.
2. Blazor posts the selected plan and billing cycle to the User Management API.
3. API activates or extends the subscription.
4. The page refreshes the current subscription summary.
