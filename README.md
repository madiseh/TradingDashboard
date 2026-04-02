# Trading Dashboard

A comprehensive trading analytics platform built with .NET, featuring COT (Commitment of Traders) data analysis and visualization capabilities.

## Introduction

Trading Dashboard is a multi-layered application following Clean Architecture principles, designed to provide real-time trading insights and data analysis. The solution consists of:

- **Blazor Web Application** - Interactive dashboard for data visualization
- **.NET MAUI Blazor Hybrid Application** - Cross-platform client for Windows, Android, and iOS
- **REST API** - Backend services for COT data and analytics
- **Clean Architecture** - Separation of Domain, Application, Infrastructure, and Presentation layers

## Getting Started

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download) or later
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/)
- .NET MAUI workloads for `android`, `ios`, and `maui-windows`
- Redis (for caching)

### Running the Application

1. **Clone the repository**
   ```bash
   git clone https://gitlab.com/Sabaghi/tradingdashboard.git
   cd TradingDashboard
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Run the API**
   ```bash
   dotnet run --project src/Presentation/TradingDashboard.Presentation.Api.Cot
   ```

4. **Run the Blazor App**
   ```bash
   dotnet run --project src/Presentation/TradingDashboard.Presentation.Blazor
   ```

5. **Run the MAUI Hybrid App**
   ```bash
   dotnet build src/Presentation/TradingDashboard.Presentation.MauiHybrid -f net9.0-windows10.0.19041.0
   ```

   For Android emulator usage, the MAUI host reads `AndroidEmulatorBaseUrl` values from `src/Presentation/TradingDashboard.Presentation.MauiHybrid/appsettings.json`.

## Build and Test

[![Pipeline Status](https://gitlab.com/Sabaghi/tradingdashboard/badges/main/pipeline.svg)](https://gitlab.com/Sabaghi/tradingdashboard/-/pipelines)

**Build the solution:**
```bash
dotnet build TradingDashboard.sln
```

**Run tests:**
```bash
dotnet test
```

View all pipelines and CI/CD status: [GitLab Pipelines](https://gitlab.com/Sabaghi/tradingdashboard/-/pipelines)

## Contribute

We welcome contributions! To get started:

### Development Environment

- **ASP.NET Core** - Familiarity with ASP.NET Core Web API and Blazor
- **Visual Studio Code** - Recommended extensions:
  - C# Dev Kit
  - C# Extensions
  - GitLens

### Project Structure

- `src/Domain/` - Core business logic and entities
- `src/Application/` - Use cases and business rules
- `src/Infrastructure/` - Data access and external services
- `src/Presentation/` - API, Blazor web UI, and MAUI Hybrid UI projects

For detailed documentation, see the [`doc/`](doc/) folder.

### Coding Standards

- Follow Clean Architecture principles
- Write unit tests for new features
- Follow the existing code style and conventions
- Keep MAUI Hybrid as a Presentation-only host and document platform-specific behavior

---

**Documentation:** [`/doc`](doc/) | **License:** MIT
