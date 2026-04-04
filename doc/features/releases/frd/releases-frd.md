# Releases FRD

## Goal

Provide a dedicated releases page that summarizes what has been added to Trading Dashboard and can be updated daily.

## Scope

- One new Blazor page at `/releases`
- Footer quick-link navigation to the releases page
- Localized page content for supported UI languages
- An initial summary entry current through April 3, 2026

## Functional Requirements

1. The application must expose a dedicated `Releases` page.
2. The footer quick links must include navigation to the releases page.
3. The page must show a concise release summary with five highlight bullets for the current snapshot.
4. The page must be structured so future daily entries can be added above existing ones.

## Non-Functional Requirements

1. The feature must stay presentation-only in its first version.
2. UI text must follow the repository localization rules.
3. The page must follow the existing Tradexy-style layout direction already used in the Blazor host.

## Acceptance Criteria

1. Users can open `/releases` directly.
2. Users can reach `/releases` from the footer quick links.
3. The page renders one current entry dated April 3, 2026 with exactly five highlights.
4. The page builds successfully with the existing Blazor project.
