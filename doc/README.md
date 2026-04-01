# TerminalPrime Documentation

This `doc` directory contains three main documentation groups:

- `governance/*`: Project-level rules, architecture direction, and repository structure decisions.
- `features/*`: Feature-level specifications, API/domain/use case docs, and FRD documents.
- `uiComponent/*`: UI component guidelines, design notes, and reusable frontend patterns.

> If a change is going to be made, this document must be traced first and nothing should be added arbitrarily.


# How To Implement Changes
- Start by reviewing `governance/*` and understand the overall implementation direction.
- If there is no existing governance document for a specific implementation topic, document the requirement and implementation plan before starting.
- Follow the documented governance process exactly.
- After governance is clear, move on to `features/*`.
- For features, use the `_template.md` file as the exact structure for new feature documentation.
- Everything must proceed step-by-step, and if required documentation is missing, do not make assumptions.