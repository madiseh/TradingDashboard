# Site Indexing Domain

## Concepts

- `Canonical Url`: the single preferred public URL for a piece of content.
- `Indexable State`: a page variation that deserves its own crawlable URL because the visible content materially changes.
- `Discovery Endpoint`: a machine-readable endpoint used by search engines to find indexable URLs.

## Rules

- Temporary UI state must not become a canonical URL.
- Query parameters are allowed only when they represent meaningful public content states.
- Historical COT date browsing is user navigation state and must not be emitted as a separate canonical sitemap URL.
