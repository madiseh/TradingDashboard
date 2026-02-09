# Common Domain

## Purpose

Define shared read-model concepts used across pages.

## Core Concepts

- `MarketType`: shared grouping enum (`Currencies`, `Cryptocurrencies`, `Indexes`, `Treasuries and Rates`, `Energies`, `Grains`, `Livestock & Dairy`, `Metals`, `Softs`).
- `MarketDirectoryGroup`: grouping object for market lists.
- `MarketDirectoryItem`: lightweight instrument projection for listing pages.
- `SupportedModel`: available models for each instrument (`Cot`, `CotLegacy`).

## Notes

- Common is read-only in phase 1.
- It provides reusable listing data for Home and future directory pages.
- No write-side behavior is required in phase 1.
