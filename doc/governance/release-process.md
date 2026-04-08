# Release Process

## Goal

Keep the public release history consistent between the Blazor UI, the running app version, and the commit history that shipped after the previous version.

## Source Of Truth

- Treat the previous value in `ApplicationVersions.Blazor` as the boundary of the last shipped release.
- Collect the new release notes from `git log <previous-release-commit>..HEAD`.
- Summarize only the work that is actually present in the shipped codebase.

## Update Steps

1. Find the commit range that starts immediately after the previous shipped version.
2. Group the commits into 3 to 5 release highlights.
3. Update `src/Presentation/Common/TradingDashboard.Presentation.Common/Constants/ApplicationVersions.cs`.
4. Add the new release entry at the top of `src/Presentation/TradingDashboard.Presentation.Blazor/Components/Pages/Releases/Releases.razor.cs`.
5. Add or update the matching resource keys in:
   - `src/Presentation/Common/TradingDashboard.Presentation.Common/Resources/Components/Pages/Releases/Releases/ReleasesResource.resx`
   - Any localized `.resx` file that overrides release page copy and dates.
6. Move the previous current release entry down and mark only the newest release as current.
7. Update the release page summary date text so it matches the newest shipped release.

## Entry Rules

- Keep entries ordered newest first.
- Use semantic versions such as `2.1.1`.
- Use one short title, one short description, and concise bullet highlights.
- Highlights may include delivery and deployment work when that work is part of the shipped release.
- Do not invent work that is not backed by the commit range.

## Verification

- Build the Blazor project after editing release resources and release entry code.
- If localization files are updated, make sure the page still resolves all release keys without missing-resource fallbacks.
