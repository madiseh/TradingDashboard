# Site Indexing Service Contract

## Responsibilities

- Collect sitemap URLs from the presentation host and upstream public APIs.
- Normalize URLs to canonical public addresses under `https://terminalprime.com`.
- De-duplicate equivalent URLs before sitemap serialization.

## Inputs

- COT instruments grouped response from the COT API.
- Currency Strength timeframe response from the Currency Strength API.

## Outputs

- Canonical absolute URLs for public pages.
- Optional `lastmod` values when trustworthy source data exists.

## Error Handling

- If an upstream API is unavailable, the sitemap must still return the stable static URLs.
- Missing or invalid upstream items must be skipped instead of failing the entire sitemap.
