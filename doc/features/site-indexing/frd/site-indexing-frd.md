# Site Indexing FRD

## Goal

Improve Google discovery and indexing quality for public TerminalPrime pages by introducing sitemap and crawler metadata support.

## Scope

- Dynamic `sitemap.xml` generation in the Blazor web host
- `robots.txt` exposure with sitemap reference
- Canonical metadata for COT report variants
- Crawlable URL states for Currency Strength timeframe variants

## Functional Requirements

1. The web host must expose `GET /sitemap.xml`.
2. The sitemap must include the home page, COT landing page, Currency Strength landing page, and public COT report pages.
3. Every COT instrument must be emitted in both `COT` and `COT Legacy` canonical forms.
4. Currency Strength timeframe variants with meaningful content differences must have stable crawlable URLs.
5. The web host must expose `GET /robots.txt` and reference the sitemap URL.
6. Pages emitted by the sitemap must render `index,follow` and self-consistent canonical URLs.

## Non-Functional Requirements

1. Sitemap generation must degrade gracefully if an upstream API is temporarily unavailable.
2. Sitemap output must remain de-duplicated and deterministic.
3. SEO URL generation must reuse shared routing constants instead of hard-coded route copies.

## Acceptance Criteria

1. Requesting `/sitemap.xml` returns valid XML.
2. The sitemap includes both `?model=cot` and `?model=cot-legacy` URLs for each available COT instrument.
3. Requesting `/robots.txt` returns a sitemap reference to the production domain.
4. Visiting a COT report page with `reportDate` still produces a canonical URL without the historical date query.
5. Visiting a Currency Strength timeframe URL preserves the selected timeframe and returns a matching canonical URL.
