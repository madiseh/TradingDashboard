# Currency Strength Domain

## Core Concepts

- `CurrencyStrengthTimeframeDefinition`: one supported source window with stable key, label, description, and query value.
- `CurrencyStrengthSnapshot`: one collected payload for one timeframe.
- `CurrencyStrengthSeries`: one currency line inside the snapshot.
- `CurrencyStrengthPoint`: one `(timestamp, value)` pair.
