# Common Use Case

## Purpose

Describe shared listing use cases for directory pages.

## Read Use Cases

### Get Market Types

- Input: none
- Output: available market types.
- API mapping: `GET /api/v1/common/market-types`

### Get Markets By Type

- Input: `type`, `q?`
- Output: market list for selected type.
- API mapping: `GET /api/v1/common/markets?type={type}`

### Get Markets Directory

- Input: none
- Output: grouped market directory for Home page.
- API mapping: `GET /api/v1/common/markets-directory`

## Validation Rules

1. Reject invalid `type`.
2. Hide model links not included in `availableModels`.
3. Hide empty groups on UI if configured.
4. Show non-blocking error when directory APIs fail.

## Presentation Client Interface

### `ICommonDirectoryClient`

- Responsibility: UI-side typed client for common directory endpoints.

Methods:
- `GetMarketsDirectoryAsync`: loads grouped market directory for Home page rendering.
