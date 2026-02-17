# Coding Rules

- Do not use fully qualified contract type names inline in code (for example: `ReadFromJsonAsync<TradingDashboard.Presentation.Blazor.Contracts.Generated.Login.ApiErrorResponse>()`).
- For generated contracts, always declare and use a file-local alias in the same file, for example:
  - `using LoginContracts = TradingDashboard.Presentation.Blazor.Contracts.Generated.Login;`
  - `ReadFromJsonAsync<LoginContracts.ApiErrorResponse>()`
