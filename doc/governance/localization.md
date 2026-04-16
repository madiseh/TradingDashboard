# Localization

Localization resources must live in the `TradingDashboard.Presentation.Common` project.

## Current languages

- `en`
- `fa`
- `tr`
- `de`

## Resource structure

Create resources per component. Do not use a shared resource file for UI text.

Examples:

- `src/Presentation/Common/TradingDashboard.Presentation.Common/Resources/Components/Layout/WorkspaceLayout/WorkspaceLayoutResource.resx`
- `src/Presentation/Common/TradingDashboard.Presentation.Common/Resources/Components/Layout/WorkspaceLayout/WorkspaceLayoutResource.fa.resx`
- `src/Presentation/Common/TradingDashboard.Presentation.Common/Resources/Components/Layout/NavMenu/NavMenuResource.resx`
- `src/Presentation/Common/TradingDashboard.Presentation.Common/Resources/Components/Layout/NavMenu/NavMenuResource.tr.resx`

Each component gets:

1. Its own folder
2. Its own marker class
3. One `.resx` file per language

## How to add a new component resource

1. Create a folder for the component under `Resources`.
2. Add a marker class for that component, for example `OrdersPageResource.cs`.
3. Add the default resource file, for example `OrdersPageResource.resx`.
4. Add one localized file per language, for example `OrdersPageResource.fa.resx`, `OrdersPageResource.tr.resx`, `OrdersPageResource.de.resx`.
5. Inject `IStringLocalizer<OrdersPageResource>` into the component.
6. Read strings with `@L["SomeKey"]`.

## Rules

- Keep keys scoped to the component.
- Add the same key to all language files.
- Use the default `.resx` file as the English fallback.
