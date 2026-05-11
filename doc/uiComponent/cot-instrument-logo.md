# COT Instrument Logo UI Component

## Component

- Name: `CotInstrumentLogo`
- Source: `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Modules/InstrumentLogo/CotInstrumentLogo.razor`

## Purpose

Render a compact visual identifier before COT instrument names in market directory tables.

## Input

- `Slug` (`string?`): instrument slug used to resolve the matching logo asset.
- `Name` (`string?`): display name used for image alt text and fallback initials.

## Behavior

- Currency instruments render a matching currency/country flag image.
- Cryptocurrency instruments render the matching crypto logo image.
- Index, rate, energy, grain, livestock/dairy, metal, and soft instruments render a dedicated image badge for the instrument slug.
- Unknown instruments render a short fallback badge generated from the instrument name or slug.

## Notes

- Must follow split-file rule: `.razor` + `.razor.cs`.
- Used by the COT directory and Markets pages so both tables identify instruments consistently.
