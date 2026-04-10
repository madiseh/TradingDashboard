# Production Deployment Layout

## Goal

Capture the currently known production server paths for TerminalPrime deployments so future deployment, restart, and environment-variable questions can be answered from one document.

## Primary Server Paths

### Environment files

Environment files are stored under:

```text
/etc/terminalprime/
```

Confirmed files visible on the server:

- `backgroundjobs.env`
- `blazor.env`
- `cot.env`
- `currency-strength.env`
- `login.env`

### Systemd service files

Systemd unit files are stored under:

```text
/etc/systemd/system/
```

Observed service files on the server:

- `terminalprime-backgroundjobs.service`
- `terminalprime-blazor.service`
- `terminalprime-cot-collect.service`
- `terminalprime-cot.service`
- `terminalprime-currency-strength-collect.service`
- `terminalprime-currency-strength.service`
- `terminalprime-login.service`

### Published application files

Published app files are deployed under:

```text
/var/www/terminalprime/
```

The current CI/CD pipeline deploys application files into these folders:

- `/var/www/terminalprime/backgroundjobs`
- `/var/www/terminalprime/blazor`
- `/var/www/terminalprime/cot`
- `/var/www/terminalprime/currency-strength`
- `/var/www/terminalprime/login`

## Known Service Mapping

| Service unit | Deploy folder | Env file | Status |
| --- | --- | --- | --- |
| `terminalprime-backgroundjobs.service` | `/var/www/terminalprime/backgroundjobs` | `/etc/terminalprime/backgroundjobs.env` | Deploy folder confirmed by CI, env filename confirmed on server, exact unit reference should be checked inside the unit file when editing. |
| `terminalprime-blazor.service` | `/var/www/terminalprime/blazor` | `/etc/terminalprime/blazor.env` | Deploy folder confirmed by CI, env filename confirmed on server, exact unit reference should be checked inside the unit file when editing. |
| `terminalprime-cot.service` | `/var/www/terminalprime/cot` | `/etc/terminalprime/cot.env` | Deploy folder confirmed by CI, env filename confirmed on server, exact unit reference should be checked inside the unit file when editing. |
| `terminalprime-currency-strength.service` | `/var/www/terminalprime/currency-strength` | `/etc/terminalprime/currency-strength.env` | Deploy folder confirmed by CI, env filename confirmed on server, exact unit reference should be checked inside the unit file when editing. |
| `terminalprime-login.service` | `/var/www/terminalprime/login` | `/etc/terminalprime/login.env` | Fully confirmed by server screenshot. |

## Confirmed Login Service Example

The `terminalprime-login.service` file shows the exact pattern used by systemd:

```ini
[Service]
WorkingDirectory=/var/www/terminalprime/login
ExecStart=/usr/bin/dotnet /var/www/terminalprime/login/TradingDashboard.Features.UserManagement.Presentation.Api.dll
EnvironmentFile=/etc/terminalprime/login.env
User=terminalprime
Group=terminalprime
```

This confirms:

1. The deployed binaries live under `/var/www/terminalprime/login`.
2. The service reads environment variables from `/etc/terminalprime/login.env`.
3. The service runs under the `terminalprime` user and group.

## CI/CD Deployment Notes

The repository deployment pipeline in `.gitlab-ci.yml` currently:

1. Publishes five deployable apps: `cot`, `currency-strength`, `login`, `backgroundjobs`, and `blazor`.
2. Uploads zip files to `/tmp/terminalprime-*.zip` on the VPS.
3. Unzips each artifact into `/var/www/terminalprime/<app-name>`.
4. Restarts the main production services:
   - `terminalprime-cot`
   - `terminalprime-backgroundjobs`
   - `terminalprime-currency-strength`
   - `terminalprime-login`
   - `terminalprime-blazor`

## Legacy / Transitional Note

The server still shows legacy collector unit files:

- `terminalprime-cot-collect.service`
- `terminalprime-currency-strength-collect.service`

Also, the CI deploy script still stops `terminalprime-cot-collect` before deploying, which suggests some collector-related services may still exist on the VPS even though the current release artifact list centers on the `backgroundjobs` host.

If a future task involves collector services, inspect the exact unit file under `/etc/systemd/system/` before changing anything.

## How To Use This Document

When a future deployment or production-support task asks:

- where env files live
- where systemd service files live
- where app binaries are deployed
- which service points to which env file

start with this document, then open the exact unit file on the server for the service being changed.
