# User Management Use Cases

## UC1: Local Sign Up

1. User submits username, email, and password.
2. API validates uniqueness.
3. API creates `AppUser`.
4. API returns JWT auth payload.

## UC2: Local Sign In

1. User submits username or email plus password.
2. API validates credentials.
3. API updates last login timestamp.
4. API returns JWT auth payload.

## UC3: Google Sign In

1. User starts Google OAuth from the login page.
2. API resolves the callback.
3. API finds or creates the matching `AppUser`.
4. API returns the user to Blazor with JWT information.

## UC4: Read Pricing Plans

1. Pricing page requests available plans.
2. API returns the premium plan catalog with monthly/yearly prices and annual savings.

## UC5: Purchase Subscription

1. Authenticated user selects monthly or yearly purchase.
2. API validates the selected plan.
3. API creates or extends the subscription.
4. API returns the updated subscription summary.
