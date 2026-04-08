# Site Indexing Use Case

## Search Engine Discovery Flow

1. Search engine requests `/robots.txt`.
2. The host advertises `/sitemap.xml`.
3. Search engine requests `/sitemap.xml`.
4. The host returns canonical public URLs for home, COT landing, COT reports, and Currency Strength states.
5. Search engine crawls each canonical page and sees `index,follow` plus matching canonical metadata.

## COT Report Discovery Flow

1. The sitemap service reads instrument slugs from the COT API.
2. The host emits one canonical URL for `COT` and one canonical URL for `COT Legacy` per instrument.
3. Historical `reportDate` URLs are excluded from sitemap canonicals.

## Currency Strength Discovery Flow

1. The sitemap service reads timeframe keys from the Currency Strength API.
2. The default timeframe stays on `/currency-strength`.
3. Non-default meaningful timeframes are emitted as query-based canonical URLs.
