# Site Indexing API Contract

## Purpose

Expose public crawl-discovery endpoints from the web presentation host so search engines can discover canonical URLs.

## Endpoints

- `GET /sitemap.xml`
  Returns XML sitemap content for public, indexable routes.
- `GET /robots.txt`
  Returns crawler rules and advertises the sitemap location.

## Rules

- Only public web URLs may be emitted.
- Auth-only routes must not be included in the sitemap.
- COT report URLs must be emitted for both `model=cot` and `model=cot-legacy`.
- Currency Strength URLs may expose query-based crawlable states when the state maps to meaningful page content.
