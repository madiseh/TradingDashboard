# Common Contract

## Purpose

Define shared listing APIs used by multiple pages, including Home directory.

## Base

- Backend base: `/api/v1/common`

## Shared Enums

### MarketType

- `Currencies`
- `Grains`
- `Cryptocurrencies`
- `Metals`
- `Energies`
- `Livestock`

## Endpoints

### 1) Get Market Types

- `GET /api/v1/common/market-types`

Response `200`:

```json
{
  "items": [
    "Currencies",
    "Grains",
    "Cryptocurrencies"
  ]
}
```

### 2) Get Markets By Type

- `GET /api/v1/common/markets?type=Grains`

Query:
- `type` (required): one of `MarketType`
- `q` (optional): search by name/code

Response `200`:

```json
{
  "type": "Grains",
  "items": [
    {
      "instrumentId": "corn-cbot-futures",
      "slug": "corn",
      "name": "Corn",
      "availableModels": ["Cot", "CotLegacy"]
    }
  ]
}
```

### 3) Get Markets Directory (Grouped)

- `GET /api/v1/common/markets-directory`

Purpose:
- Return grouped list for Home page (`Currencies`, `Grains`, `Cryptocurrencies`, ...).

Response `200`:

```json
{
  "groups": [
    {
      "type": "Currencies",
      "items": []
    },
    {
      "type": "Grains",
      "items": []
    },
    {
      "type": "Cryptocurrencies",
      "items": []
    }
  ]
}
```

## Navigation Contract (for COT pages)

- `COT`: `/cot/{instrumentSlug}?model=Cot`
- `COT Legacy`: `/cot/{instrumentSlug}?model=CotLegacy`
