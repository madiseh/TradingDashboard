# Currency Strength Use Cases

## UC1: Read Supported Timeframes

1. User opens the Currency Strength page.
2. Blazor requests `/api/v1/currency-strength/timeframes`.
3. API returns the ordered timeframe buttons.
4. UI selects the default timeframe and requests its latest snapshot.

## UC2: Read Latest Snapshot

1. User selects a timeframe.
2. Blazor requests `/api/v1/currency-strength/timeframes/{timeframeKey}/latest`.
3. API reads the latest stored file for the timeframe.
4. UI renders all currency lines on the chart.

## UC3: Collect Snapshots

1. Background jobs host triggers the Currency Strength collect job every 1 minute.
2. Collector fetches each configured URL.
3. Collector validates the JSON shape.
4. Collector writes `latest.json` and a timestamped snapshot file.

## UC4: Filter Visible Currencies

1. User opens the Currency Strength chart.
2. UI marks all available currency legend chips as selected by default.
3. User selects a currency chip.
4. UI renders only the selected currency series.
5. User selects additional currency chips.
6. UI renders the selected currency series together.
