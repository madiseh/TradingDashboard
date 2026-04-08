# Pricing Domain

## Core Concepts

### `PricingPlan`

- `PlanCode`
- `PlanName`
- `MonthlyPriceUsd`
- `YearlyPriceUsd`
- `YearlyDiscountPercentage`
- `Description`
- `TrialHeadline`
- `Features[]`

### `PricingSelection`

- `PlanCode`
- `BillingCycle` (`Monthly`, `Yearly`)

## Rules

1. The page must always present monthly and yearly options for the same plan.
2. Yearly purchase must show a discount compared to twelve monthly payments.
3. UI text stays fully localized and is not embedded directly in the component.
