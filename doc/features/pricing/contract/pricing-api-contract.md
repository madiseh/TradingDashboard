# Pricing API Contract

## Purpose

The Pricing page is a public Blazor page that reads plan metadata from the User Management API and can trigger a subscription purchase for authenticated users.

## Consumed Endpoints

- `GET /api/v1/login/subscriptions/plans`
- `GET /api/v1/login/subscriptions/me`
- `POST /api/v1/login/subscriptions/purchase`

## Read Plans

- `GET /api/v1/login/subscriptions/plans`
- Auth: not required

Response shape:

```json
[
  {
    "planCode": "premium",
    "planName": "TerminalPrime Premium",
    "monthlyPriceUsd": 149,
    "yearlyPriceUsd": 1490,
    "yearlyDiscountPercentage": 17,
    "trialHeadline": "10-day guided onboarding",
    "description": "Full access to TerminalPrime market intelligence.",
    "features": [
      "COT market coverage",
      "Macro analysis",
      "Economic calendar"
    ]
  }
]
```

## Read Current Subscription

- `GET /api/v1/login/subscriptions/me`
- Auth: bearer token required

## Purchase Subscription

- `POST /api/v1/login/subscriptions/purchase`
- Auth: bearer token required

Request shape:

```json
{
  "planCode": "premium",
  "billingCycle": "yearly"
}
```

Response shape:

```json
{
  "subscriptionId": "0d5fd8d8-1b02-4ee6-9c43-1d67aef06d80",
  "userId": "97ca9093-d5f0-49b8-8f7d-d74b3ef3e5bd",
  "planCode": "premium",
  "planName": "TerminalPrime Premium",
  "billingCycle": "yearly",
  "status": "active",
  "basePriceUsd": 1788,
  "discountPercentage": 17,
  "finalPriceUsd": 1490,
  "startsAtUtc": "2026-04-08T13:30:00Z",
  "expiresAtUtc": "2027-04-08T13:30:00Z"
}
```
