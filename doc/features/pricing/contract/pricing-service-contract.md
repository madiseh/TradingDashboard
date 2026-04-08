# Pricing Service Contract

## Page Inputs

- Localized resource keys for all visible texts.
- Plan catalog response from the User Management API.
- Optional authenticated subscription summary for the signed-in user.

## Page Responsibilities

1. Expose a dedicated `/pricing` route.
2. Render monthly and yearly purchase options for the same plan.
3. Highlight yearly savings.
4. Redirect unauthenticated users to the login page before purchase.
5. Trigger subscription purchase when the user is authenticated.

## UI Rules

- The page must use a dedicated resource marker and one `.resx` file per supported language.
- The top menu must contain a direct `Pricing` entry.
- The CTA state must reflect whether the user is already subscribed.
