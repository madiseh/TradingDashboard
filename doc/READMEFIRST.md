# TerminalPrime Documentation

This `doc` directory contains three main documentation groups:

- `governance/*`: Project-level rules, architecture direction, repository structure decisions, and release authoring rules.
- `features/*`: Feature-level specifications, API/domain/use case docs, and FRD documents.
- `uiComponent/*`: UI component guidelines, design notes, and reusable frontend patterns.

> If a change is going to be made, this document must be traced first and nothing should be added arbitrarily.


# How To Implement Changes
- Start by reviewing `governance/*` and understand the overall implementation direction.
- For production deployment paths, env file locations, systemd unit locations, and server folder mapping, review `governance/production-deployment-layout.md`.
- For release work, review `governance/release-process.md` before updating version numbers or release entries.
- If there is no existing governance document for a specific implementation topic, document the requirement and implementation plan before starting.
- Follow the documented governance process exactly.
- After governance is clear, move on to `features/*`.
- For features, use the `_template.md` file as the exact structure for new feature documentation.
- Unit tests are mandatory for every feature change. No feature is complete unless it has unit test coverage.
- Before implementing or finishing any feature work, always review `test/TradingDashboard.UnitTests/*` and verify whether coverage already exists for the related feature behavior.
- For any change under `src/Features/*/*Application/Features/*`, add or update the matching test under `test/TradingDashboard.UnitTests/Features/*`.
- If the required feature project is not referenced by `test/TradingDashboard.UnitTests/TradingDashboard.UnitTests.csproj`, add the needed `ProjectReference` first and then add the unit tests.
- Everything must proceed step-by-step, and if required documentation is missing, do not make assumptions.


# Step-by-step implementation
1. When creating a new page, first verify whether a new UI component is required.
1.1. If a new UI component is needed, document it under `uiComponent/*`.
1.2. Then add a new module for that component inside `TradingDashboard.Presentation.Blazor` under `Components -> Modules`, making it reusable for others.
1.3. For each new UI page, which will likely use several existing modules or components, create a Resource file. Use the `Localization.md` documentation for the resource setup.
